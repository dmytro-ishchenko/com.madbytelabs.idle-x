#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MadByteLabs.After.Editor
{
    /// <summary>
    /// Exports world-space terrain, road mesh triangles, build-point footprints and perimeter mesh data.
    /// Read-only: does not modify the scene or TerrainData.
    /// Open via Tools > A.F.T.E.R. > Export Terrain Data.
    /// </summary>
    public sealed class AfterTerrainDataExporter : EditorWindow
    {
        [SerializeField] private Terrain terrain;
        [SerializeField] private Transform roadsRoot;
        [SerializeField] private Transform buildPointsRoot;
        [SerializeField] private Transform perimeterRoot;
        [SerializeField] private bool includeInactive = true;
        [SerializeField] private bool exportRoadMeshTriangles = true;
        [SerializeField] private bool exportPerimeterMeshTriangles = false;

        [MenuItem("Tools/A.F.T.E.R./Terrain/Export Terrain Data")]
        private static void Open()
        {
            var window = GetWindow<AfterTerrainDataExporter>("AFTER Terrain Export");
            window.minSize = new Vector2(420, 300);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Terrain Painter / Scene Data Export", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Drag scene objects into the fields below. Export is read-only. " +
                "Roads are exported as world-space mesh triangles (not approximated centerlines). " +
                "Build points should be direct children of Build Points Root.", MessageType.Info);

            terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);
            roadsRoot = (Transform)EditorGUILayout.ObjectField("Roads Root", roadsRoot, typeof(Transform), true);
            buildPointsRoot = (Transform)EditorGUILayout.ObjectField("Build Points Root", buildPointsRoot, typeof(Transform), true);
            perimeterRoot = (Transform)EditorGUILayout.ObjectField("Perimeter Root (optional)", perimeterRoot, typeof(Transform), true);
            includeInactive = EditorGUILayout.Toggle("Include Inactive", includeInactive);
            exportRoadMeshTriangles = EditorGUILayout.Toggle("Road Mesh Triangles", exportRoadMeshTriangles);
            exportPerimeterMeshTriangles = EditorGUILayout.Toggle("Perimeter Mesh Triangles", exportPerimeterMeshTriangles);

            EditorGUILayout.Space(10);
            using (new EditorGUI.DisabledScope(terrain == null || terrain.terrainData == null))
            {
                if (GUILayout.Button("Export JSON", GUILayout.Height(34))) Export();
            }

            if (terrain == null) EditorGUILayout.HelpBox("Assign a Terrain first.", MessageType.Warning);
            if (roadsRoot == null) EditorGUILayout.HelpBox("Assign Roads Root to include road geometry.", MessageType.Warning);
            if (buildPointsRoot == null) EditorGUILayout.HelpBox("Assign Build Points Root to include footprints.", MessageType.Warning);
        }

        private void Export()
        {
            string path = EditorUtility.SaveFilePanel("Save AFTER terrain data", "", "AFTER_TerrainSceneData", "json");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                var data = terrain.terrainData;
                var scene = new SceneExport
                {
                    formatVersion = 1,
                    sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                    unityVersion = Application.unityVersion,
                    exportedUtc = DateTime.UtcNow.ToString("o"),
                    coordinateSystem = "Unity world coordinates; XZ plane for surface positions; Y is height; units = meters",
                    terrain = new TerrainExport
                    {
                        name = terrain.name,
                        position = terrain.transform.position,
                        size = data.size,
                        heightmapResolution = data.heightmapResolution,
                        alphamapResolution = data.alphamapResolution,
                        detailResolution = data.detailResolution,
                        layers = ReadTerrainLayers(data)
                    },
                    roads = roadsRoot != null ? ReadMeshes(roadsRoot, exportRoadMeshTriangles) : Array.Empty<MeshExport>(),
                    buildPoints = buildPointsRoot != null ? ReadBuildPoints(buildPointsRoot) : Array.Empty<BuildPointExport>(),
                    perimeter = perimeterRoot != null ? ReadMeshes(perimeterRoot, exportPerimeterMeshTriangles) : Array.Empty<MeshExport>()
                };

                File.WriteAllText(path, JsonUtility.ToJson(scene, true));
                Debug.Log($"[AFTER Terrain Export] Saved: {path}. Roads: {scene.roads.Length}, " +
                          $"build points: {scene.buildPoints.Length}, perimeter meshes: {scene.perimeter.Length}");
                EditorUtility.DisplayDialog("AFTER Terrain Export",
                    $"Export complete.\n\nRoad meshes: {scene.roads.Length}\nBuild points: {scene.buildPoints.Length}\nPerimeter meshes: {scene.perimeter.Length}\n\n{path}", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                EditorUtility.DisplayDialog("Export failed", ex.Message, "OK");
            }
        }

        private static TerrainLayerExport[] ReadTerrainLayers(TerrainData data)
        {
            var result = new List<TerrainLayerExport>();
            foreach (var layer in data.terrainLayers)
            {
                if (layer == null) continue;
                result.Add(new TerrainLayerExport
                {
                    name = layer.name,
                    diffuseTextureAssetPath = layer.diffuseTexture != null ? AssetDatabase.GetAssetPath(layer.diffuseTexture) : "",
                    tileSize = layer.tileSize,
                    tileOffset = layer.tileOffset,
                    metallic = layer.metallic,
                    smoothness = layer.smoothness
                });
            }
            return result.ToArray();
        }

        private MeshExport[] ReadMeshes(Transform root, bool includeTriangles)
        {
            var result = new List<MeshExport>();
            foreach (var filter in root.GetComponentsInChildren<MeshFilter>(includeInactive))
            {
                if (filter.sharedMesh == null) continue;
                if (!includeInactive && !filter.gameObject.activeInHierarchy) continue;
                var mesh = filter.sharedMesh;
                var export = new MeshExport
                {
                    name = filter.name,
                    hierarchyPath = GetPath(filter.transform),
                    localToWorldMatrix = MatrixToArray(filter.transform.localToWorldMatrix),
                    worldBoundsCenter = filter.GetComponent<Renderer>() != null
                        ? filter.GetComponent<Renderer>().bounds.center
                        : filter.transform.TransformPoint(mesh.bounds.center),
                    worldBoundsSize = filter.GetComponent<Renderer>() != null
                        ? filter.GetComponent<Renderer>().bounds.size : TransformBounds(filter.transform, mesh.bounds).size,
                    vertexCount = mesh.vertexCount,
                    triangleCount = CountTriangles(mesh)
                };
                if (includeTriangles)
                {
                    Vector3[] vertices = mesh.vertices;
                    export.worldVertices = new Vector3[vertices.Length];
                    for (int i = 0; i < vertices.Length; i++)
                        export.worldVertices[i] = filter.transform.TransformPoint(vertices[i]);
                    export.triangles = ReadAllTriangles(mesh);
                }
                else
                {
                    export.worldVertices = Array.Empty<Vector3>();
                    export.triangles = Array.Empty<int>();
                }
                result.Add(export);
            }
            return result.ToArray();
        }

        private BuildPointExport[] ReadBuildPoints(Transform root)
        {
            var result = new List<BuildPointExport>();
            foreach (Transform point in root)
            {
                if (!includeInactive && !point.gameObject.activeInHierarchy) continue;
                Bounds localBounds;
                string footprintSource;
                var box = point.GetComponent<BoxCollider>();
                if (box != null)
                {
                    localBounds = new Bounds(box.center, box.size);
                    footprintSource = "Root BoxCollider";
                }
                else if (TryCombinedBounds(point, out Bounds worldBounds))
                {
                    // Renderer bounds are axis-aligned in world space; corners are exported directly below.
                    result.Add(new BuildPointExport
                    {
                        name = point.name,
                        hierarchyPath = GetPath(point),
                        position = point.position,
                        rotationEuler = point.eulerAngles,
                        worldFootprintCorners = XZCorners(worldBounds),
                        footprintSource = "Combined world renderer/collider AABB (approximate)",
                        approximate = true
                    });
                    continue;
                }
                else
                {
                    // Explicit fallback; should be corrected by adding a root BoxCollider matching the pad.
                    localBounds = new Bounds(Vector3.zero, new Vector3(20f, 0f, 20f));
                    footprintSource = "Fallback 20x20m (approximate)";
                }

                result.Add(new BuildPointExport
                {
                    name = point.name,
                    hierarchyPath = GetPath(point),
                    position = point.position,
                    rotationEuler = point.eulerAngles,
                    worldFootprintCorners = OrientedXZCorners(point, localBounds),
                    footprintSource = footprintSource,
                    approximate = box == null
                });
            }
            return result.ToArray();
        }

        private static bool TryCombinedBounds(Transform root, out Bounds bounds)
        {
            bool found = false;
            bounds = new Bounds(root.position, Vector3.zero);
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                if (!found) { bounds = collider.bounds; found = true; }
                else bounds.Encapsulate(collider.bounds);
            }
            if (found) return true;
            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                if (!found) { bounds = renderer.bounds; found = true; }
                else bounds.Encapsulate(renderer.bounds);
            }
            return found;
        }

        private static Vector3[] OrientedXZCorners(Transform transform, Bounds local)
        {
            var c = local.center;
            var e = local.extents;
            return new[]
            {
                transform.TransformPoint(c + new Vector3(-e.x, 0, -e.z)),
                transform.TransformPoint(c + new Vector3(-e.x, 0,  e.z)),
                transform.TransformPoint(c + new Vector3( e.x, 0,  e.z)),
                transform.TransformPoint(c + new Vector3( e.x, 0, -e.z))
            };
        }

        private static Vector3[] XZCorners(Bounds b)
        {
            var c = b.center; var e = b.extents;
            return new[]
            {
                c + new Vector3(-e.x, 0, -e.z), c + new Vector3(-e.x, 0, e.z),
                c + new Vector3(e.x, 0, e.z), c + new Vector3(e.x, 0, -e.z)
            };
        }

        private static Bounds TransformBounds(Transform t, Bounds b)
        {
            var c = t.TransformPoint(b.center);
            var x = t.TransformVector(b.extents.x, 0, 0);
            var y = t.TransformVector(0, b.extents.y, 0);
            var z = t.TransformVector(0, 0, b.extents.z);
            var e = new Vector3(Mathf.Abs(x.x) + Mathf.Abs(y.x) + Mathf.Abs(z.x),
                                Mathf.Abs(x.y) + Mathf.Abs(y.y) + Mathf.Abs(z.y),
                                Mathf.Abs(x.z) + Mathf.Abs(y.z) + Mathf.Abs(z.z));
            return new Bounds(c, e * 2f);
        }

        private static int CountTriangles(Mesh mesh)
        {
            int n = 0;
            for (int i = 0; i < mesh.subMeshCount; i++)
                if (mesh.GetTopology(i) == MeshTopology.Triangles) n += (int)mesh.GetIndexCount(i) / 3;
            return n;
        }

        private static int[] ReadAllTriangles(Mesh mesh)
        {
            var all = new List<int>();
            for (int i = 0; i < mesh.subMeshCount; i++)
                if (mesh.GetTopology(i) == MeshTopology.Triangles)
                    all.AddRange(mesh.GetTriangles(i));
            return all.ToArray();
        }

        private static float[] MatrixToArray(Matrix4x4 matrix)
        {
            var a = new float[16];
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++) a[r * 4 + c] = matrix[r, c];
            return a;
        }

        private static string GetPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null) { t = t.parent; path = t.name + "/" + path; }
            return path;
        }

        [Serializable] private class SceneExport
        {
            public int formatVersion;
            public string sceneName;
            public string unityVersion;
            public string exportedUtc;
            public string coordinateSystem;
            public TerrainExport terrain;
            public MeshExport[] roads;
            public BuildPointExport[] buildPoints;
            public MeshExport[] perimeter;
        }
        [Serializable] private class TerrainExport
        {
            public string name;
            public Vector3 position;
            public Vector3 size;
            public int heightmapResolution;
            public int alphamapResolution;
            public int detailResolution;
            public TerrainLayerExport[] layers;
        }
        [Serializable] private class TerrainLayerExport
        {
            public string name;
            public string diffuseTextureAssetPath;
            public Vector2 tileSize;
            public Vector2 tileOffset;
            public float metallic;
            public float smoothness;
        }
        [Serializable] private class MeshExport
        {
            public string name;
            public string hierarchyPath;
            public float[] localToWorldMatrix;
            public Vector3 worldBoundsCenter;
            public Vector3 worldBoundsSize;
            public int vertexCount;
            public int triangleCount;
            public Vector3[] worldVertices;
            public int[] triangles;
        }
        [Serializable] private class BuildPointExport
        {
            public string name;
            public string hierarchyPath;
            public Vector3 position;
            public Vector3 rotationEuler;
            public Vector3[] worldFootprintCorners;
            public string footprintSource;
            public bool approximate;
        }
    }
}
#endif
