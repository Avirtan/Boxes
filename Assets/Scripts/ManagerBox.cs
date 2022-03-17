using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ManagerBox : MonoBehaviour
{
    [SerializeField]
    private List<Box> _allBox;
    [SerializeField]
    private Box _selectedBox;
    [SerializeField]
    private Box_event _box_Event;
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private float _moveSpeed;
    [SerializeField]
    private Vector3 _startPosition;
    [SerializeField]
    private bool _onStartPosition;
    [SerializeField]
    private AnimationCurve _animationCurve;
    [SerializeField]
    private float _deltaTime;
    [SerializeField]
    private Transform _nextBox;

    void Start()
    {
        _allBox = new List<Box>();
        foreach (var box in FindObjectsOfType<Box>())
        {
            _allBox.Add(box);
            int randRotateX = Random.Range(0, 360);
            int randRotateY = Random.Range(0, 360);
            box.transform.rotation = Quaternion.Euler(new Vector3(randRotateX, randRotateY, 0));
        }
        _box_Event = FindObjectOfType<Box_event>();
        _startPosition = _box_Event.transform.position;

    }

    void Update()
    {
        _deltaTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            _deltaTime = 0;
            _onStartPosition = false;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.transform.gameObject.GetComponent<Box>())
                {
                    _selectedBox = hit.transform.gameObject.GetComponent<Box>();
                    _nextBox = CalculateNextBox();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _deltaTime = 0;
            _onStartPosition = true;
            _selectedBox = null;
        }
        var currentBox = _box_Event.gameObject.transform;
        if (_selectedBox)
        {
            var currentPos = Vector3.MoveTowards(currentBox.position, _nextBox.transform.position, _moveSpeed * Time.deltaTime);
            if (currentPos == _nextBox.transform.position && _nextBox.position != _selectedBox.transform.position)
            {
                _nextBox = CalculateNextBox();
                _deltaTime = 0;
            }
            currentPos.y += _animationCurve.Evaluate(_deltaTime);
            currentBox.position = currentPos;
            currentBox.rotation = Quaternion.Slerp(currentBox.rotation, _nextBox.rotation, _rotationSpeed * Time.deltaTime);
        }
        if (_onStartPosition)
        {
            currentBox.position = Vector3.MoveTowards(currentBox.position, _startPosition, _moveSpeed * Time.deltaTime);
            currentBox.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private Transform CalculateNextBox()
    {
        var currentPosition = _box_Event.transform.position;
        var finalPosition = _selectedBox.transform.position;
        var distance = 0f;
        var minDistance = Vector3.Distance(currentPosition, finalPosition);
        var boxReturn = _selectedBox;
        for (var i = 0; i < _allBox.Count; i++)
        {
            var box = _allBox[i];
            var boxPosition = box.transform.position;
            if (boxPosition == finalPosition) continue;
            distance = Vector3.Distance(boxPosition, currentPosition);
            if (Vector3.Distance(boxPosition, finalPosition) <
                Vector3.Distance(currentPosition, finalPosition))
            {
                if (distance < minDistance && Mathf.Round(distance) != 0)
                {
                    minDistance = distance;
                    boxReturn = box;
                }
            }
        }
        return boxReturn.transform;
    }
}
