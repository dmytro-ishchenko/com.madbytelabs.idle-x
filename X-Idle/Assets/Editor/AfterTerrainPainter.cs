// AFTER Terrain Painter v0.3 — Unity 6 Editor-only. Place in Assets/Editor/.
// Reads AFTER_TerrainSceneData.json exported by AfterTerrainDataExporter.
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AfterTerrainPainter : EditorWindow
{
    [Serializable] class P { public float x, y, z; }
    [Serializable] class TerrainInfo { public P position, size; public int alphamapResolution; }
    [Serializable] class MeshInfo { public string name; public P worldBoundsCenter, worldBoundsSize; public P[] worldVertices; public int[] triangles; }
    [Serializable] class PointInfo { public string name; public P position; public P[] worldFootprintCorners; public bool approximate; }
    [Serializable] class SceneInfo { public TerrainInfo terrain; public MeshInfo[] roads, perimeter; public PointInfo[] buildPoints; }

    Terrain target;
    TextAsset source;
    float blendWidth = 13f;
    float roadBlendMin = 3f;
    float roadBlendMax = 17f;
    float roadEdgeNoiseSize = 29f;
    float roadEdgeVariation = 1f;
    float macroSize = 75f;
    float macroContrast = 1.5f;
    float roadCompaction = 0.85f;
    float buildingInfluence = 0.35f;
    bool showAdvanced = true;
    Vector2 scroll;
    enum PreviewMode { FinalColors, MacroZones, RoadInfluence, BuildingInfluence, GrassWeight, SoilWeight, IndustrialWeight, CompactedWeight }
    PreviewMode previewMode = PreviewMode.FinalColors;
    float[,] debugMacro, debugRoad, debugBuilding;

    float roadShoulderWidth = 12f;
    float industrialRadius = 48f;
    float industrialStrength = 0.50f;
    float roadsideContrast = 1.65f;
    float patchStrength = 1.4f;
    float patchSize = 42f;
    float noiseScale = 0.012f;
    int seed = 1257;
    Texture2D preview;
    float[,,] generated;
    SceneInfo parsed;
    Color[] palette = {
        Hex("#85845C"), // Grass
        Hex("#A48B62"), // Dry soil
        Hex("#827367"), // Industrial
        Hex("#766451")  // Compacted roadside
    };

    [MenuItem("Tools/A.F.T.E.R./Terrain Painter")]
    public static void ShowWindow() => GetWindow<AfterTerrainPainter>("AFTER Terrain Painter");

    void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.LabelField("AFTER Terrain Painter v0.3", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Flat Synty palette; large macro zones; coherent randomized road shoulders. Generate Preview before Apply.", MessageType.Info);
        EditorGUI.BeginChangeCheck();
        target = (Terrain)EditorGUILayout.ObjectField("Terrain", target, typeof(Terrain), true);
        source = (TextAsset)EditorGUILayout.ObjectField("Export JSON", source, typeof(TextAsset), false);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Ground Colors", EditorStyles.boldLabel);
        string[] colorNames={"Dry Grass / Olive","Dry Soil / Sand","Industrial / Gray Brown","Compacted / Roadside"};
        for(int i=0;i<4;i++) palette[i]=EditorGUILayout.ColorField(colorNames[i],palette[i]);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Macro Zones",EditorStyles.boldLabel);
        macroSize=EditorGUILayout.Slider("Macro Zone Size (m)",macroSize,25f,180f);
        macroContrast=EditorGUILayout.Slider("Macro Zone Contrast",macroContrast,0.5f,3f);
        patchStrength=EditorGUILayout.Slider("Secondary Patch Variation",patchStrength,0f,3f);
        patchSize=EditorGUILayout.Slider("Secondary Patch Size (m)",patchSize,12f,110f);
        industrialRadius=EditorGUILayout.Slider("HQ Industrial Radius (m)",industrialRadius,10f,110f);
        industrialStrength=EditorGUILayout.Slider("HQ Industrial Strength",industrialStrength,0f,1f);
        buildingInfluence=EditorGUILayout.Slider("Building Influence",buildingInfluence,0f,1f);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Roadside / Organic Border",EditorStyles.boldLabel);
        roadShoulderWidth=EditorGUILayout.Slider("Roadside Base Width (m)",roadShoulderWidth,2f,24f);
        roadsideContrast=EditorGUILayout.Slider("Roadside Contrast",roadsideContrast,0f,4f);
        roadCompaction=EditorGUILayout.Slider("Compacted Ground Strength",roadCompaction,0f,2f);
        roadBlendMin=EditorGUILayout.Slider("Blend Distance MIN (m)",roadBlendMin,0.5f,30f);
        roadBlendMax=EditorGUILayout.Slider("Blend Distance MAX (m)",roadBlendMax,0.5f,45f);
        roadEdgeNoiseSize=EditorGUILayout.Slider("Edge Noise Scale (m)",roadEdgeNoiseSize,6f,110f);
        roadEdgeVariation=EditorGUILayout.Slider("Edge Randomization",roadEdgeVariation,0f,1f);
        EditorGUILayout.HelpBox("MIN/MAX set the random transition distance measured outward from the road mesh edge. Low-frequency noise varies it smoothly along each road; no pixel-by-pixel randomness.",MessageType.None);

        showAdvanced=EditorGUILayout.Foldout(showAdvanced,"Advanced",true);
        if(showAdvanced) {
            blendWidth=EditorGUILayout.Slider("Building Blend Width (m)",blendWidth,2f,35f);
            noiseScale=EditorGUILayout.Slider("Detail Noise Frequency",noiseScale,0.002f,0.04f);
            seed=EditorGUILayout.IntField("Random Seed",seed);
        }
        previewMode=(PreviewMode)EditorGUILayout.EnumPopup("Preview Mode",previewMode);
        bool changed=EditorGUI.EndChangeCheck();
        if(changed) {generated=null;debugMacro=null;debugRoad=null;debugBuilding=null; if(preview) {DestroyImmediate(preview);preview=null;}}
        if(roadBlendMax < roadBlendMin) EditorGUILayout.HelpBox("MAX must be greater than or equal to MIN. The generator will clamp it.",MessageType.Warning);
        if(!target||!source){EditorGUILayout.HelpBox("Assign Terrain and exported JSON imported as TextAsset.",MessageType.Info);EditorGUILayout.EndScrollView();return;}
        if(GUILayout.Button("Generate Preview",GUILayout.Height(28))) Generate();
        using(new EditorGUI.DisabledScope(generated==null)) {
            if(GUILayout.Button("Apply To Terrain (Undo supported)",GUILayout.Height(28))) Apply();
            if(GUILayout.Button("Save Preview PNG")) SavePreview();
        }
        if(preview) {
            float w=Mathf.Max(64f,Mathf.Min(position.width-35f,512f));
            Rect r=GUILayoutUtility.GetRect(w,w);
            EditorGUI.DrawPreviewTexture(new Rect(r.x,r.y,w,w),preview,null,ScaleMode.ScaleToFit);
            EditorGUILayout.HelpBox("Preview uses generated alpha weights. Scene mesh roads/buildings are not drawn. Change Preview Mode and regenerate for debug views.",MessageType.None);
        }
        EditorGUILayout.EndScrollView();
    }

    static Color Hex(string hex) { ColorUtility.TryParseHtmlString(hex, out Color c); return c; }
    static Vector2 XZ(P p) => new Vector2(p.x, p.z);
    static float Smooth(float a, float b, float v) => Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(a, b, v));
    static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b) {
        Vector2 ab = b - a;
        return Vector2.Distance(p, a + ab * Mathf.Clamp01(Vector2.Dot(p-a, ab) / Mathf.Max(0.00001f, ab.sqrMagnitude)));
    }
    static bool InsideTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c) {
        float s1 = Cross(b-a,p-a), s2 = Cross(c-b,p-b), s3 = Cross(a-c,p-c);
        return (s1 >= -0.0001f && s2 >= -0.0001f && s3 >= -0.0001f) ||
               (s1 <= 0.0001f && s2 <= 0.0001f && s3 <= 0.0001f);
    }
    static float Cross(Vector2 a, Vector2 b) => a.x*b.y-a.y*b.x;

    void Generate()
    {
        try { parsed = JsonUtility.FromJson<SceneInfo>(source.text); }
        catch (Exception ex) { Debug.LogException(ex); return; }
        if (parsed == null || parsed.terrain == null || parsed.terrain.position == null || parsed.terrain.size == null || parsed.roads == null || parsed.buildPoints == null) {
            EditorUtility.DisplayDialog("Invalid JSON", "Terrain, roads or buildPoints missing.", "OK"); return;
        }
        TerrainData td = target.terrainData;
        if (td == null) return;
        Vector3 origin = target.transform.position;
        Vector3 size = td.size;
        Vector3 exported = new Vector3(parsed.terrain.position.x,parsed.terrain.position.y,parsed.terrain.position.z);
        Vector3 exportedSize = new Vector3(parsed.terrain.size.x,parsed.terrain.size.y,parsed.terrain.size.z);
        if ((origin-exported).sqrMagnitude > 0.1f || (size-exportedSize).sqrMagnitude > 0.1f) {
            EditorUtility.DisplayDialog("Terrain mismatch", "Export JSON and selected Terrain have different position or size. Re-export from this scene.", "OK"); return;
        }
        int n = td.alphamapResolution;
        bool[,] road = new bool[n,n];
        // Rasterize actual road mesh triangles in XZ, including intersections.
        foreach (MeshInfo mesh in parsed.roads) {
            if (mesh.worldVertices == null || mesh.triangles == null) continue;
            P[] verts=mesh.worldVertices; int[] tris=mesh.triangles;
            for (int i=0; i+2<tris.Length; i+=3) {
                int ia=tris[i],ib=tris[i+1],ic=tris[i+2];
                if (ia<0||ib<0||ic<0||ia>=verts.Length||ib>=verts.Length||ic>=verts.Length) continue;
                Vector2 a=XZ(verts[ia]), b=XZ(verts[ib]), c=XZ(verts[ic]);
                if (Mathf.Abs(Cross(b-a,c-a)) < 0.0001f) continue;
                int x0=Mathf.Clamp(Mathf.FloorToInt((Mathf.Min(a.x,Mathf.Min(b.x,c.x))-origin.x)/size.x*n)-1,0,n-1);
                int x1=Mathf.Clamp(Mathf.CeilToInt((Mathf.Max(a.x,Mathf.Max(b.x,c.x))-origin.x)/size.x*n)+1,0,n-1);
                int y0=Mathf.Clamp(Mathf.FloorToInt((Mathf.Min(a.y,Mathf.Min(b.y,c.y))-origin.z)/size.z*n)-1,0,n-1);
                int y1=Mathf.Clamp(Mathf.CeilToInt((Mathf.Max(a.y,Mathf.Max(b.y,c.y))-origin.z)/size.z*n)+1,0,n-1);
                for(int y=y0;y<=y1;y++) for(int x=x0;x<=x1;x++) {
                    Vector2 p = new Vector2(origin.x + (x+0.5f)*size.x/n,origin.z+(y+0.5f)*size.z/n);
                    if (InsideTriangle(p,a,b,c)) road[y,x]=true;
                }
            }
        }
        // 8-neighbor chamfer distance in world meters, outward from road footprint.
        float[,] dist = new float[n,n]; float dx=size.x/n, dy=size.z/n, diag=Mathf.Sqrt(dx*dx+dy*dy);
        for(int y=0;y<n;y++)for(int x=0;x<n;x++)dist[y,x]=road[y,x]?0f:1e7f;
        for(int y=0;y<n;y++) for(int x=0;x<n;x++) {
            float v=dist[y,x]; if(x>0)v=Mathf.Min(v,dist[y,x-1]+dx);
            if(y>0) {v=Mathf.Min(v,dist[y-1,x]+dy);if(x>0)v=Mathf.Min(v,dist[y-1,x-1]+diag);if(x<n-1)v=Mathf.Min(v,dist[y-1,x+1]+diag);} dist[y,x]=v;
        }
        for(int y=n-1;y>=0;y--)for(int x=n-1;x>=0;x--){
            float v=dist[y,x]; if(x<n-1)v=Mathf.Min(v,dist[y,x+1]+dx);
            if(y<n-1){v=Mathf.Min(v,dist[y+1,x]+dy);if(x>0)v=Mathf.Min(v,dist[y+1,x-1]+diag);if(x<n-1)v=Mathf.Min(v,dist[y+1,x+1]+diag);} dist[y,x]=v;
        }
        // Compound boundary reconstructed as convex hull of perimeter mesh centers.
        List<Vector2> wall = new List<Vector2>();
        if(parsed.perimeter!=null) foreach(MeshInfo m in parsed.perimeter) if(m.worldBoundsCenter!=null) wall.Add(XZ(m.worldBoundsCenter));
        List<Vector2> hull=Hull(wall);
        Vector2 center=Vector2.zero; int count=0;
        foreach(PointInfo pt in parsed.buildPoints) {center+=XZ(pt.position); count++;} if(count>0)center/=count;
        // Cache footprints to avoid polygon allocations on every alphamap pixel.
        List<Vector2[]> footprints = new List<Vector2[]>();
        foreach (PointInfo bp in parsed.buildPoints) {
            if (bp.worldFootprintCorners == null || bp.worldFootprintCorners.Length < 3) continue;
            Vector2[] poly = new Vector2[bp.worldFootprintCorners.Length];
            for (int i = 0; i < poly.Length; i++) poly[i] = XZ(bp.worldFootprintCorners[i]);
            footprints.Add(poly);
        }
        generated=new float[n,n,4];
        debugMacro=new float[n,n];debugRoad=new float[n,n];debugBuilding=new float[n,n];
        Color[] pixels=new Color[n*n];
        for(int y=0;y<n;y++) for(int x=0;x<n;x++) {
            Vector2 p=new Vector2(origin.x+(x+0.5f)*dx,origin.z+(y+0.5f)*dy);
            // Large smooth macro zones: use separate coherent fields to avoid a single dominant soil weight.
            float bigScale=1f/Mathf.Max(20f,macroSize);
            float m0=Mathf.PerlinNoise(p.x*bigScale+seed*0.0137f+37.4f,p.y*bigScale+seed*0.0081f+11.7f);
            float m1=Mathf.PerlinNoise(p.x*bigScale*0.63f+129.4f,p.y*bigScale*0.63f+seed*0.013f);
            float macro=Mathf.Clamp01((0.72f*m0+0.28f*m1-0.5f)*macroContrast+0.5f);
            float invPatch=1f/Mathf.Max(8f,patchSize);
            float nA=Mathf.PerlinNoise(p.x*invPatch+seed*0.017f,p.y*invPatch+seed*0.031f);
            float nB=Mathf.PerlinNoise(p.x*invPatch*0.49f+17.87f,p.y*invPatch*0.49f+89.31f);
            float nC=Mathf.PerlinNoise(p.x*noiseScale+111.6f,p.y*noiseScale+seed*0.021f);
            float patches=(nA-0.5f)*0.7f+(nB-0.5f)*0.5f+(nC-0.5f)*0.22f;
            float inside=hull.Count>=3&&PointInPolygon(p,hull)?1f:0f;
            float centerDistance=Vector2.Distance(p,center);
            float centerZone=1f-Smooth(industrialRadius*0.30f,industrialRadius*1.75f,
                centerDistance+(nB-0.5f)*12f);

            // Coherent random shoulder width in meters at the local world position.
            // This makes the outer edge wander naturally along the road without white noise.
            float edgeFreq=1f/Mathf.Max(6f,roadEdgeNoiseSize);
            float e0=Mathf.PerlinNoise(p.x*edgeFreq+seed*0.097f+71.3f,p.y*edgeFreq+44.2f);
            float e1=Mathf.PerlinNoise(p.x*edgeFreq*0.46f+7.1f,p.y*edgeFreq*0.46f+seed*0.063f);
            float randomT=Mathf.Clamp01(e0*0.75f+e1*0.25f);
            randomT=Mathf.Lerp(0.5f,randomT,roadEdgeVariation);
            float localBlend=Mathf.Lerp(roadBlendMin,Mathf.Max(roadBlendMin,roadBlendMax),randomT);
            float roadDistance=dist[y,x];
            // Core shoulder from mesh edge, then variable soft transition.
            float coreEnd=Mathf.Max(0.1f,roadShoulderWidth*0.36f);
            float fadeEnd=roadShoulderWidth+localBlend;
            float roadInfluence=1f-Smooth(coreEnd,fadeEnd,roadDistance);
            roadInfluence=Mathf.Clamp01(roadInfluence);
            debugMacro[y,x]=macro;
            debugRoad[y,x]=roadInfluence;

            float nearBuilding = 0f;
            foreach (Vector2[] bp in footprints) {
                float dd = DistanceToPolygon(p, bp);
                nearBuilding = Mathf.Max(nearBuilding,
                    1f - Smooth(1.0f, 7f + blendWidth * 0.55f, dd));
            }

            debugBuilding[y,x]=nearBuilding;
            // Independent macro weights with soft transitions, then local modifiers.
            // Three broad color families occupy substantial space even inside the fence.
            float oliveZone=1f-Smooth(0.39f,0.63f,macro+patches*patchStrength*0.25f);
            float soilZone=1f-Mathf.Abs(macro-0.52f)/0.31f;
            soilZone=Mathf.Clamp01(soilZone);
            float grayZone=Smooth(0.43f,0.70f,macro+patches*patchStrength*0.30f);
            float grass=0.12f+oliveZone*1.65f;
            float soil=0.14f+soilZone*1.38f;
            float industrial=0.08f+grayZone*1.25f;
            float compact=0.015f;
            if(inside>0.5f) {
                industrial+=industrialStrength*centerZone*1.3f;
                industrial+=nearBuilding*buildingInfluence*0.7f;
                compact+=roadInfluence*roadsideContrast*roadCompaction*1.8f;
                // Suppress bright grass in the shoulder but retain patch variations beyond it.
                grass*=1f-0.88f*roadInfluence;
                soil*=1f-0.66f*roadInfluence;
                industrial*=1f-0.30f*roadInfluence;
            } else {
                grass*=1.65f;soil*=0.6f;industrial*=0.35f;
                compact*=0.05f;
                // Allow optional recoloring outside walls; otherwise preserve existing weights on Apply.
            }
            float[] weights={Mathf.Max(0.01f,grass),Mathf.Max(0.01f,soil),Mathf.Max(0.01f,industrial),Mathf.Max(0.01f,compact)};
            float sum=weights[0]+weights[1]+weights[2]+weights[3];
            Color col=Color.black;
            for(int k=0;k<4;k++){float w=weights[k]/sum;generated[y,x,k]=w;col+=palette[k]*w;}
            switch(previewMode) {
                case PreviewMode.MacroZones: col=Color.Lerp(palette[0],palette[2],macro);break;
                case PreviewMode.RoadInfluence: col=Color.Lerp(Color.black,Color.white,roadInfluence);break;
                case PreviewMode.BuildingInfluence: col=Color.Lerp(Color.black,Color.white,nearBuilding);break;
                case PreviewMode.GrassWeight: col=Color.Lerp(Color.black,Color.white,generated[y,x,0]);break;
                case PreviewMode.SoilWeight: col=Color.Lerp(Color.black,Color.white,generated[y,x,1]);break;
                case PreviewMode.IndustrialWeight: col=Color.Lerp(Color.black,Color.white,generated[y,x,2]);break;
                case PreviewMode.CompactedWeight: col=Color.Lerp(Color.black,Color.white,generated[y,x,3]);break;
            }
            pixels[y*n+x]=col;
        }
        if(preview)DestroyImmediate(preview);
        preview=new Texture2D(n,n,TextureFormat.RGBA32,false,true);
        preview.SetPixels(pixels);preview.Apply(false);Repaint();
        Debug.Log($"AFTER Painter: {parsed.roads.Length} road meshes, {parsed.buildPoints.Length} build points, {wall.Count} perimeter parts. Generated {n}x{n} preview.");
    }

    static float DistanceToPolygon(Vector2 p, Vector2[] poly) {
        float min = 1e8f;
        for (int i = 0; i < poly.Length; i++)
            min = Mathf.Min(min, DistanceToSegment(p, poly[i], poly[(i + 1) % poly.Length]));
        // AABB footprint is approximate, but treating its interior as zero gives a useful halo.
        bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++) {
            Vector2 a = poly[i], b = poly[j];
            if ((a.y > p.y) != (b.y > p.y) &&
                p.x < (b.x - a.x) * (p.y - a.y) / (b.y - a.y) + a.x) inside = !inside;
        }
        return inside ? 0f : min;
    }
    static bool PointInPolygon(Vector2 p,List<Vector2> poly){
        bool inside=false;for(int i=0,j=poly.Count-1;i<poly.Count;j=i++) {
            Vector2 a=poly[i],b=poly[j];
            if((a.y>p.y)!=(b.y>p.y) && p.x<(b.x-a.x)*(p.y-a.y)/(b.y-a.y)+a.x)inside=!inside;
        }return inside;
    }
    static List<Vector2> Hull(List<Vector2> pts) {
        pts.Sort((a,b)=>a.x==b.x?a.y.CompareTo(b.y):a.x.CompareTo(b.x));
        if(pts.Count<3)return pts;
        List<Vector2> h=new List<Vector2>();
        foreach(Vector2 p in pts){while(h.Count>=2&&Cross(h[h.Count-1]-h[h.Count-2],p-h[h.Count-1])<=0)h.RemoveAt(h.Count-1);h.Add(p);}
        int lower=h.Count;
        for(int i=pts.Count-2;i>=0;i--){Vector2 p=pts[i];while(h.Count>lower&&Cross(h[h.Count-1]-h[h.Count-2],p-h[h.Count-1])<=0)h.RemoveAt(h.Count-1);h.Add(p);}
        h.RemoveAt(h.Count-1);return h;
    }

    void Apply() {
        if(generated==null||!target)return;
        TerrainData td=target.terrainData;
        if(td.alphamapResolution!=generated.GetLength(0)) {Debug.LogError("Terrain resolution changed; regenerate preview.");return;}
        if(!EditorUtility.DisplayDialog("Apply Terrain Paint", "Change Terrain Layers and alphamaps? Use Undo to revert. Save a scene/project backup first.", "Apply", "Cancel")) return;
        string folder="Assets/AFTER/TerrainGenerated";
        EnsureFolder("Assets/AFTER");EnsureFolder(folder);
        TerrainLayer[] layers=new TerrainLayer[4];
        string[] names={"DryGrass","DrySoil","Industrial","Compacted"};
        for(int i=0;i<4;i++) {
            string texPath=$"{folder}/{names[i]}_Flat.png", layerPath=$"{folder}/{names[i]}.terrainlayer";
            {
                Texture2D t=new Texture2D(4,4,TextureFormat.RGBA32,false);
                Color[] pixels=new Color[16];for(int j=0;j<16;j++)pixels[j]=palette[i];
                t.SetPixels(pixels);t.Apply();File.WriteAllBytes(texPath,t.EncodeToPNG());DestroyImmediate(t);
                AssetDatabase.ImportAsset(texPath,ImportAssetOptions.ForceUpdate);
            }
            TextureImporter ti=AssetImporter.GetAtPath(texPath) as TextureImporter;
            if(ti!=null) {ti.wrapMode=TextureWrapMode.Repeat;ti.textureCompression=TextureImporterCompression.Uncompressed;ti.mipmapEnabled=true;ti.SaveAndReimport();}
            TerrainLayer layer=AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath);
            if(layer==null){layer=new TerrainLayer();AssetDatabase.CreateAsset(layer,layerPath);}
            layer.diffuseTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            layer.tileSize=new Vector2(16,16);EditorUtility.SetDirty(layer);layers[i]=layer;
        }
        Undo.RegisterCompleteObjectUndo(td,"AFTER Terrain Paint");
        td.terrainLayers=layers;
        td.SetAlphamaps(0,0,generated);
        EditorUtility.SetDirty(td);AssetDatabase.SaveAssets();
        Debug.Log("AFTER Terrain painted. Existing road meshes, building objects and Terrain heights were not modified.");
    }
    static void EnsureFolder(string folder){if(AssetDatabase.IsValidFolder(folder))return;int k=folder.LastIndexOf('/');AssetDatabase.CreateFolder(folder.Substring(0,k),folder.Substring(k+1));}
    void SavePreview(){if(!preview)return;string p=EditorUtility.SaveFilePanel("Save Terrain Preview",Application.dataPath,"AFTER_TerrainPreview","png");if(!string.IsNullOrEmpty(p))File.WriteAllBytes(p,preview.EncodeToPNG());}
    void OnDisable(){if(preview)DestroyImmediate(preview);}
}
