using UnityEngine;

public class ZoomCanvas : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    public Canvas Canvas => _canvas;

    public static ZoomCanvas Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
