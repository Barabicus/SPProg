using UnityEngine;
using System.Collections;

public class TextureCollision : SpellEffect
{
    public Renderer renderer;
    Texture2D mainTex;

    protected override void Start()
    {
        base.Start();
        mainTex = new Texture2D(50, 50);
        Color[] colors = new Color[50 * 50];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.red;
        }

        mainTex.SetPixels(colors);
        mainTex.Apply();

        renderer.material.mainTexture = mainTex;
    }

    protected override void effectSetting_OnSpellCollision(ColliderEventArgs args, Collider obj)
    {
        base.effectSetting_OnSpellCollision(args, obj);
        Ray ray = new Ray(obj.transform.position, (transform.position - obj.transform.position).normalized);
        RaycastHit hit;
        if (GetComponent<Collider>().Raycast(ray, out hit, 500f))
        {
            Debug.DrawRay(hit.point, Vector3.up * 5f, Color.green, 5f);

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= mainTex.width;
            pixelUV.y *= mainTex.height;
            Color[] white = new Color[20 * 20];
            for(int i = 0; i < white.Length; i++)
                white[i] = Color.white;

            mainTex.SetPixels((int)pixelUV.x, (int)pixelUV.y, 5, 5, white);
            mainTex.Apply();
            renderer.material.mainTexture = mainTex;

        }

    }

}
