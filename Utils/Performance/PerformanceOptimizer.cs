using System;
using System.Collections;
using System.Collections.Generic;
using Replay.Utils;
using UnityEngine;

public class PerformanceOptimizer : ComponentSingleton<PerformanceOptimizer>
{
    [Header("Warmup Cameras")]
    [SerializeField] public List<SceneWarmupCamera> warmupCameras;
    
    [Header("Shaders")]
    [SerializeField] public bool optimizeShadersOnStartAsync = true;
    [SerializeField] ShaderVariantCollection shaderVariants;
    [SerializeField] public int warmupVariantsPerFrame = 10;
    
    [Header("Particles")]
    [SerializeField] public bool warmupParticlesOnAwake = true;
    [SerializeField] public List<GameObject> particlesToWarmup;
    [SerializeField] public List<GameObject> particlePrefabsToWarmup;

    private void Awake()
    {
        if (warmupParticlesOnAwake)
            WarmupParticles();
            
    }

    void Start()
    {
        if (optimizeShadersOnStartAsync)
            WarmupShaderVariantsAsync();
    }
    #region Warmup Cameras
    public void BlitWarmupCameras(Action onComplete = null)
    {
        if (warmupCameras != null)
        {
            foreach (var warmupCamera in warmupCameras)
                warmupCamera.Blit(onComplete);    
        }else
            onComplete?.Invoke();
    }
    #endregion

    #region Particles

    public void WarmupParticles()
    {
        foreach (var particle in particlesToWarmup)
            PreloadParticleEffectGameObject(particle);
        foreach (var particle in particlePrefabsToWarmup)
            PreloadParticleEffectFromPrefab(particle);
    }

    

    public void PreloadParticleEffectFromPrefab(GameObject particleEffectPrefab)
    {
        // Instantiate the prefab but keep it disabled
        var instance = Instantiate(particleEffectPrefab);
        instance.SetActive(false);

        PreloadParticleEffectGameObject(instance);

        // Destroy the instance after a short delay
        Destroy(instance, 0.1f);
    }
    public void PreloadParticleEffectGameObject(GameObject particleEffectGO)
    {
        
        // Force resource loading
        var particleSystems = particleEffectGO.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            var mRenderer = ps.GetComponent<ParticleSystemRenderer>();
            if (mRenderer != null && mRenderer.material != null)
            {
                var main = ps.main;
                main.prewarm = true;
                // Access the material to force loading
                var mat = mRenderer.material;
                if (mat != null)
                {
                    
                }
            }
        }
    }

    #endregion

    #region Shaders
    public void WarmupAllShaders()
    {
        Shader.WarmupAllShaders();
    }

    public void WarmupShaderVariants()
    {
        if (shaderVariants != null)
        {
            // Warm up all shader variants in the collection
            shaderVariants.WarmUp();
        }
    }
    
    public void WarmupShaderVariantsAsync(Action onComplete = null)
    {
        if (shaderVariants != null)
        {
            // Start progressive warmup
            StartCoroutine(WarmUpShadersProgressively(onComplete));
        }else
            onComplete?.Invoke();
    }

    private IEnumerator WarmUpShadersProgressively(Action onComplete)
    {
        int totalVariants = shaderVariants.variantCount;
        int warmedUpVariants = 0;

        Debug.Log($"PerformanceOptimizer: Starting shader warmup: {totalVariants} total variants");

        while (warmedUpVariants < totalVariants)
        {
            // WarmUpProgressively returns true when all variants are warmed up
            bool isComplete = shaderVariants.WarmUpProgressively(warmupVariantsPerFrame);

            warmedUpVariants = shaderVariants.warmedUpVariantCount;
            Debug.Log($"PerformanceOptimizer: Warmed up {warmedUpVariants}/{totalVariants} shader variants");

            if (!isComplete)
                break;

            yield return null; // Wait for next frame
        }
        Debug.Log($"PerformanceOptimizer: Warmup complete: {warmedUpVariants}/{totalVariants} shader variants");

        onComplete?.Invoke();
    }
    #endregion
}
