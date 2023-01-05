using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//The DragDrop Class is the parent Class of ItemSlot,
//it contains all the main functions that the ItemSlot needs to be moved around the InventoryUI;
public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [HideInInspector]
    public GameObject equipmentParentTransform;

    private Canvas _canvas;

    private Image         _image;
    private RectTransform _rectTransform;
    public InventorySlot _parentSlot;

    private Vector3 _startingPosition;
    private bool _dropped = false;


    //Getters
    public Vector3 startingPosition
    { get { return _startingPosition; } set { _startingPosition = value; } }

    public Image image
    { get { return _image; } set { image = value; } }

    public RectTransform rectTransform
    { get { return _rectTransform; } set { rectTransform = value; } }

    public InventorySlot parentSlot
    { get { return _parentSlot; } set { _parentSlot = value; } }

    public bool dropped
    { get { return _dropped; } set { _dropped = value; } }

    //This values need to be cached before the Start function fires;
    private void Awake()
    {
        equipmentParentTransform = GameObject.Find("Character");
        _rectTransform = GetComponent<RectTransform>();
        _image         = GetComponent<Image>();
    }

    protected virtual void Start()
    {
        _canvas     = GetComponentInParent<Canvas>();
        _parentSlot = GetComponentInParent<InventorySlot>();
        _startingPosition = _rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dropped = false;
        _image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Set the parent to an higher object in the hierarchy in order to avoid dragging it below other objects;
        if(equipmentParentTransform)
        transform.SetParent(equipmentParentTransform.transform);

        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _image.raycastTarget = true;

        //Return the object to its starting position if it hasn't been moved to another slot;
        if (!dropped)
        {
            _rectTransform.position = _startingPosition;
             transform.SetParent(_parentSlot.transform);
        }
    }
}
