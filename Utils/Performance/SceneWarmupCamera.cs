using System;
using Replay.Utils;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SceneWarmupCamera : MonoBehaviour
{
    [Header("Events and Behavior")]
    [SerializeField] private bool snapsOnAwake = false;
    [SerializeField] private bool snapsOnStart = true;
    [SerializeField] private bool createsRenderTextureOnAwake = true;
    [SerializeField] private bool destroysRenderTextureImmediately = true;
    
    [Header("Camera")]
    [SerializeField] private Camera warmupCamera;
    
    [Header("Render Texture")]
    [SerializeField] private int renderTextureWidth = 100;
    [SerializeField] private int renderTextureHeight = 100;
    [SerializeField] private int renderTextureDepth = 24;
    [SerializeField] private RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;
    
    //private
    private RenderTexture _tempRT = null;
    private void Awake()
    {
        if(warmupCamera == null)
            warmupCamera = GetComponent<Camera>();
        
        if(createsRenderTextureOnAwake)
            CreateRenderTextureIfNeeded();
        
        warmupCamera.enabled = false;
        
        if(snapsOnAwake)
            Blit();
    }

    void Start()
    {
        if(snapsOnStart)
            Blit();
    }

    private void OnDestroy()
    {
        DestroyRenderTextureIfNeeded();
    }

    #region Camera
    public void Blit(Action onComplete = null)
    {
        warmupCamera.enabled = true;
        
        CreateRenderTextureIfNeeded();
        
        //allow one frame to render
        this.NextFrame(() =>
        {
            // Release the temporary render texture
            if (destroysRenderTextureImmediately)
                DestroyRenderTextureIfNeeded();
            
            warmupCamera.enabled = false;
            onComplete?.Invoke();
        });
    }

    public void CreateRenderTextureIfNeeded(){
        if (_tempRT == null)
        {
            _tempRT = RenderTexture.GetTemporary(
                renderTextureWidth, 
                renderTextureHeight, 
                renderTextureDepth, renderTextureFormat);
            warmupCamera.targetTexture = _tempRT;
        }
    }

    public void DestroyRenderTextureIfNeeded(){
        if (_tempRT != null)
        {
            warmupCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(_tempRT);
            _tempRT = null;
        }
    }
#endregion
}
