using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class GameEnvironment : ScriptableObject
{
    private const string assetName = "GameSettings";
    private const string assetPath = "Resources";
    private const string assetExtension = ".asset";

    private static GameEnvironment _instance;

    public static GameEnvironment Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load(assetName) as GameEnvironment;
                if (_instance == null)
                {
                    _instance = CreateInstance<GameEnvironment>();
#if UNITY_EDITOR
                    var properPath = Path.Combine(Application.dataPath, assetPath);

                    if (!Directory.Exists(properPath))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                    }

                    var fullPath = Path.Combine(Path.Combine("Assets", assetPath), assetName + assetExtension);
                    AssetDatabase.CreateAsset(_instance, fullPath);
#endif
                }
            }
            return _instance;
        }
    }

    [SerializeField] private string _stagingServer;
    [SerializeField] private string _productionServer;
    [SerializeField] private string _activeServer;
    [SerializeField] private string _versionName;
    [SerializeField] private string _releaseNumber;
    [SerializeField] private int _currentReleaseNumber;
    [SerializeField] private bool _isProductionBuild;
    
    public string StagingServer
    {
        get { return Instance._stagingServer; }
        set
        {
            if (Instance._stagingServer != value)
            {
                Instance._stagingServer = value;
                DirtyEditor();
            }
        }
    }
    
    public string ProductionServer
    {
        get { return Instance._productionServer; }
        set
        {
            if (Instance._productionServer != value)
            {
                Instance._productionServer = value;
                DirtyEditor();
            }
        }
    }

    public string ActiveServer
    {
        get { return Instance._activeServer; }
        set
        {
            if (Instance._activeServer != value)
            {
                Instance._activeServer = value;
                DirtyEditor();
            }
        }
    }

    public string VersionName
    {
        get { return Instance._versionName; }
        set
        {
            if (Instance._versionName != value)
            {
                Instance._versionName = value;
                DirtyEditor();
            }
        }
    }

    public string ReleaseNumber
    {
        get { return Instance._releaseNumber; }
        set
        {
            if (Instance._releaseNumber != value)
            {
                Instance._releaseNumber = value;
                DirtyEditor();
            }
        }
    }

    public int CurrentReleaseNumber
    {
        get { return Instance._currentReleaseNumber; }
        set
        {
            if (Instance._currentReleaseNumber != value)
            {
                Instance._currentReleaseNumber = value;
                DirtyEditor();
            }
        }
    }
    
    public bool IsProductionBuild {
        get { return Instance._isProductionBuild; }
        set {
            if (Instance._isProductionBuild != value) {
                Instance._isProductionBuild = value;
                DirtyEditor();
            }
        }
    }

    private static void DirtyEditor()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(Instance);
        AssetDatabase.SaveAssets();
#endif
    }
}