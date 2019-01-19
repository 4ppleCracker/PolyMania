using UnityEngine;

class Skin
{
    private static Skin m_currentlyLoaded;
    public static Skin CurrentlyLoadedSkin {
        get {
            return m_currentlyLoaded;
        }
        set {
            m_currentlyLoaded = value;
        }
    }
    static Skin()
    {
        // Load default skin
        CurrentlyLoadedSkin = new Skin
        {
            SliceTexture = Resources.Load<Texture2D>("Textures/SliceTexture")
        };
    }
    public void Apply()
    {
        PolyMesh.Instance.meshRenderer.material.mainTexture = CurrentlyLoadedSkin.SliceTexture;
    }

    public Texture2D SliceTexture;
}
