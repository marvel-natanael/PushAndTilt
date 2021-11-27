using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.5f;

    private MeshRenderer parallaxObj;

    public float ScrollSpeed { get => scrollSpeed; set => scrollSpeed = value; }

    private void Awake()
    {
        parallaxObj = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        var textureOffset = new Vector2(Time.time * -scrollSpeed, 0);
        parallaxObj.material.mainTextureOffset = textureOffset;
    }
}