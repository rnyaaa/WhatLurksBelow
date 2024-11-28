using UnityEngine;
 
public class CustomImageEffect : MonoBehaviour {
 
    public Material material;
 
    [ExecuteInEditMode]
    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Debug.Log("BLIT");
        Graphics.Blit(src, dest, material);
    }
}