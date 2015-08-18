using UnityEngine;
using System.Collections;

public class MovingBackground : MonoBehaviour {

    public Material stock, koth, race, settings, quit, black;

    private Renderer renderer;
    private int selectedOption;
    private float offsetXSpeed = 1f;
    private float offsetYSpeed = 1f;
    private Vector2 offset;
    private int prevOption;

	// Use this for initialization
	void Awake ()
    {
        renderer = GetComponent<Renderer>();

        offset = renderer.material.GetTextureOffset("_MainTex");

        setOption(-1);
	}

    void OnDisable()
    {
        setOption(-1);
    }

    void OnEnable()
    {
        setOption(-1);
    }

	// Update is called once per frame
    void Update()
    {
        setOffset();
    }

    private void setOffset()
    {
        float scale = 20;

        if (selectedOption != prevOption)
        {
            prevOption = selectedOption;

            switch (selectedOption)
            {
                case 0:
                    renderer.material = stock;
                    offsetXSpeed = -1.25f * scale;
                    offsetYSpeed = -1 * scale;
                    break;
                case 1:
                    renderer.material = race;
                    offsetXSpeed = 1 * scale;
                    offsetYSpeed = -2 * scale;
                    break;
                case 2:
                    renderer.material = koth;
                    offsetXSpeed = 1.5f * scale;
                    offsetYSpeed = 1 * scale;
                    break;
                case 3:
                    renderer.material = settings;
                    offsetXSpeed = 1.5f * scale;
                    offsetYSpeed = -1 * scale;
                    break;
                case 4:
                    renderer.material = quit;
                    offsetXSpeed = -1 * scale;
                    offsetYSpeed = 1.5f * scale;
                    break;
                default:
                    renderer.material = black;
                    offsetXSpeed = 1f;
                    offsetYSpeed = 1f;
                    break;
            }
        }

        offset.x += (0.05f * offsetXSpeed * Time.deltaTime);
        offset.y += (0.05f * offsetYSpeed * Time.deltaTime);

        if (offset.x > 1)
            offset.x -= 1;

        if (offset.x < -1)
            offset.x += 1;

        if (offset.y > 1)
            offset.y -= 1;
                   
        if (offset.y < -1)
            offset.y += 1;

        renderer.material.SetTextureOffset("_MainTex", offset);
    }

    private void resetOffset()
    {
        offset = Vector2.zero;
    }

    public void setOption(int option)
    {
        if (selectedOption != option)
        {
            selectedOption = option;
            resetOffset();

            float offsetXSpeed = 0;
            float offsetYSpeed = 0;

            setOffset();

            switch (selectedOption)
            {
                case 0:
                    offsetXSpeed = -1.25f;
                    offsetYSpeed = -1;
                    break;
                case 1:
                    offsetXSpeed = 1;
                    offsetYSpeed = -2;
                    break;
                case 2:
                    offsetXSpeed = 1.5f;
                    offsetYSpeed = 1;
                    break;
                case 3:
                    offsetXSpeed = -1f;
                    offsetYSpeed = 1.5f;
                    break;
                case 4:
                    offsetXSpeed = 1.5f;
                    offsetYSpeed = -1;
                    break;
                default:
                    offsetXSpeed = 0;
                    offsetYSpeed = 0;
                    break;
            }

            transform.rotation = Quaternion.Euler(new Vector3(90 + (15 * offsetXSpeed), 10 * offsetYSpeed, 0));
        }
    }
}
