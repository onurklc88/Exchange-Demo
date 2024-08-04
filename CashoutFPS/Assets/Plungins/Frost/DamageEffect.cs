using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Frost")]
public class DamageEffect : MonoBehaviour
{
    private float _damageAmount = 0f;
    public float EdgeSharpness = 1; 
    public float minFrost = 0; //0-1
    public float maxFrost = 1; //0-1
    public float seethroughness = 0.2f; 
    public float distortion = 0.1f; 
    public Texture2D Frost; 
    public Texture2D FrostNormals;
    public Shader Shader; 
	private Material material;
    private bool _increase;
    private float _damageStrength;


	private void Awake()
	{
        material = new Material(Shader);
        material.SetTexture("_BlendTex", Frost);
        material.SetTexture("_BumpMap", FrostNormals);
	}

    private void OnEnable()
    {
       PlayerHealth.OnDamageEffectNeeded += PlayScreenDamage;
    }

    private void OnDisable()
    {
        PlayerHealth.OnDamageEffectNeeded -= PlayScreenDamage;
    }


    private void Start()
    {
        _damageAmount = 0;
    }

  
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!Application.isPlaying)
        {
            material.SetTexture("_BlendTex", Frost);
            material.SetTexture("_BumpMap", FrostNormals);
            EdgeSharpness = Mathf.Max(1, EdgeSharpness);
        }
        material.SetFloat("_BlendAmount", Mathf.Clamp01(Mathf.Clamp01(_damageAmount) * (maxFrost - minFrost) + minFrost));
        material.SetFloat("_EdgeSharpness", EdgeSharpness);
        material.SetFloat("_SeeThroughness", seethroughness);
        material.SetFloat("_Distortion", distortion);
        //Debug.Log("_Distortion: "+ distortion);

		Graphics.Blit(source, destination, material);
	}

   private void PlayScreenDamage(bool condition, float damageValue)
   {
       _increase = condition;
       _damageStrength = damageValue;
        StartCoroutine(IncreaseOrDecreaseDamageAmount());
   }


    private IEnumerator IncreaseOrDecreaseDamageAmount()
    {
     
        if (_increase)
        {
            if(_damageStrength > 30f)
            {
                _damageAmount += 0.1f;
            }
            else
            {
                _damageAmount += 0.02f;
            }
           
        }
        else
        {
            while (_damageAmount > 0)
            {
               
                _damageAmount -= 0.5f * Time.deltaTime;

                yield return null; 
            }
            _damageAmount = 0; 
        }
    }

   
}