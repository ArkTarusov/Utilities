using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AiryCat.Utilities.Helper;
using UnityEngine.EventSystems;

namespace AiryCat.Utilities.InputHelper
{
    public enum Swipe
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
        Click
    }

    public class SwipeManager : Singleton<SwipeManager>
    {

        #region Settings
        [Tooltip("Min swipe distance")] [SerializeField] private float _minSwipeLength = 0.5f;

        [Tooltip("If false, swipe triger at the end of the swipe")]
        [SerializeField]
        private bool _triggerSwipeAtMinLength;

        [Tooltip("Whether to detect eight or four cardinal directions")]
        [SerializeField]
        private bool _useEightDirections;
        #endregion

        #region Auxiliary const parameters
        private const float EIGHT_DIR_ANGLE = 0.906f;
        private const float FOUR_DIR_ANGLE = 0.5f;
        private const float DEFAULT_DPI = 72f;
        private const float DPCM_FACTOR = 2.54f;

        private static class DirectionVector
        {
            public static readonly Vector2 Up = Vector2.up;
            public static readonly Vector2 Down = Vector2.down;
            public static readonly Vector2 Right = Vector2.right;
            public static readonly Vector2 Left = Vector2.left;
            public static readonly Vector2 UpRight = new Vector2(1, 1);
            public static readonly Vector2 UpLeft = new Vector2(-1, 1);
            public static readonly Vector2 DownRight = new Vector2(1, -1);
            public static readonly Vector2 DownLeft = new Vector2(-1, -1);
        }
        private static readonly Dictionary<Swipe, Vector2> DirectionVectors = new Dictionary<Swipe, Vector2>()
        {
            {Swipe.Up, DirectionVector.Up},
            {Swipe.Down, DirectionVector.Down},
            {Swipe.Right, DirectionVector.Right},
            {Swipe.Left, DirectionVector.Left},
            {Swipe.UpRight, DirectionVector.UpRight},
            {Swipe.UpLeft, DirectionVector.UpLeft},
            {Swipe.DownRight, DirectionVector.DownRight},
            {Swipe.DownLeft, DirectionVector.DownLeft}
        };
        #endregion

        #region Create Events

        private static void CheckNullEvents()
        {
            if (_onSwipeDetectedVelocity == null && _onSwipeDetectedVector == null && _onClickDetected == null
                && _onSwipeDetected==null)
                _autoDetectSwipes = false;

        }

        public delegate void OnSwipeDetectedVelocityHandler(Swipe swipeDirection, Vector2 swipeVelocity);
        private static OnSwipeDetectedVelocityHandler _onSwipeDetectedVelocity;
        public event OnSwipeDetectedVelocityHandler OnSwipeDetectedVelocity
        {
            add
            {
                _onSwipeDetectedVelocity += value;
                _autoDetectSwipes = true;
            }
            remove
            {
                if (_onSwipeDetectedVelocity == null) return;
                _onSwipeDetectedVelocity -= value;
                CheckNullEvents();
            }
        }

        public delegate void OnSwipeDetectedHandler(Swipe swipeDirection);
        private static OnSwipeDetectedHandler _onSwipeDetected;
        public event OnSwipeDetectedHandler OnSwipeDetected
        {
            add
            {
                _onSwipeDetected += value;
                _autoDetectSwipes = true;
            }
            remove
            {
                if (_onSwipeDetected == null) return;
                _onSwipeDetected -= value;
                CheckNullEvents();
            }
        }

        public delegate void OnSwipeDetectedVectorHandler(Swipe swipeDirection, Vector2 swipeVector);
        private static OnSwipeDetectedVectorHandler _onSwipeDetectedVector;
        public event OnSwipeDetectedVectorHandler OnSwipeDetectedVector
        {
            add
            {
                _onSwipeDetectedVector += value;
                _autoDetectSwipes = true;
            }
            remove
            {
                if (_onSwipeDetectedVector == null) return;
                _onSwipeDetectedVector -= value; CheckNullEvents();
            }
        }

        public delegate void OnClickDetectedHandler();
        private static OnClickDetectedHandler _onClickDetected;
        public event OnClickDetectedHandler OnClickDetected
        {
            add
            {
                _onClickDetected += value;
                _autoDetectSwipes = true;
            }
            remove
            {
                if (_onClickDetected == null) return;
                _onClickDetected -= value; CheckNullEvents();
            }
        }
        #endregion

        #region isSwipe Bool
        public bool IsSwiping() { return _swipeDirection != Swipe.None; }
        public bool IsSwipingRight() { return IsSwipingDirection(Swipe.Right); }
        public bool IsSwipingLeft() { return IsSwipingDirection(Swipe.Left); }
        public bool IsSwipingUp() { return IsSwipingDirection(Swipe.Up); }
        public bool IsSwipingDown() { return IsSwipingDirection(Swipe.Down); }
        public bool IsSwipingDownLeft() { return IsSwipingDirection(Swipe.DownLeft); }
        public bool IsSwipingDownRight() { return IsSwipingDirection(Swipe.DownRight); }
        public bool IsSwipingUpLeft() { return IsSwipingDirection(Swipe.UpLeft); }
        public bool IsSwipingUpRight() { return IsSwipingDirection(Swipe.UpRight); }
        public bool IsClick() { return IsSwipingDirection(Swipe.Click); }

        private static bool IsSwipingDirection(Swipe swipeDir)
        {
            DetectionSwipe();
            return _swipeDirection == swipeDir;
        }
        #endregion

        private static Vector2 _swipeVelocity;
        private static float _dpcm;
        private static float _swipeStartTime;
        private static float _swipeEndTime;
        private static bool _autoDetectSwipes;
        private static bool _swipeEnded;
        private static Swipe _swipeDirection;
        private static Vector2 _firstPressPos;
        private static Vector2 _secondPressPos;


        private void Start()
        {
            var dpi = Math.Abs(Screen.dpi) < 0.1f ? DEFAULT_DPI : Screen.dpi;
            _dpcm = dpi / DPCM_FACTOR;
        }

        private void Update()
        {
            if (_autoDetectSwipes)
            {
                DetectionSwipe();
            }
        }

        private static void DetectionSwipe()
        {
            if (GetTouchInput() || GetMouseInput())
            {
                if (_swipeEnded)
                {
                    return;
                }

                var currentSwipe = _secondPressPos - _firstPressPos;
                var swipeCm = currentSwipe.magnitude / _dpcm;
                
                if (swipeCm < Instance._minSwipeLength)
                {
                    if (Instance._triggerSwipeAtMinLength) return;
                    if (Application.isEditor)
                    {
                        Debug.Log("[SwipeManager] Swipe was not long enough.");
                    }

                    _swipeDirection = Swipe.Click;
                    _onClickDetected?.Invoke();
                    return;
                }

                _swipeEndTime = Time.time;
                _swipeVelocity = currentSwipe * (_swipeEndTime - _swipeStartTime);
                _swipeDirection = GetSwipeDirByTouch(currentSwipe);
                _swipeEnded = true;

                _onSwipeDetected?.Invoke(_swipeDirection);
                _onSwipeDetectedVelocity?.Invoke(_swipeDirection, _swipeVelocity);
                _onSwipeDetectedVector?.Invoke(_swipeDirection,DirectionVectors[_swipeDirection]);
            }
            else
            {
                _swipeDirection = Swipe.None;
            }
        }
        
        private static bool GetTouchInput()
        {
            if (Input.touches.Length <= 0) return false;
            var t = Input.GetTouch(0);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (IsPointerOverUiObject()) break;
                    _firstPressPos = t.position;
                    _swipeStartTime = Time.time;
                    _swipeEnded = false;
                    break;
                case TouchPhase.Ended:
                    _secondPressPos = t.position;
                    return true;
                default:
                    if (Instance._triggerSwipeAtMinLength)
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        private static bool GetMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUiObject()) return false;
                _firstPressPos = Input.mousePosition;
                _swipeStartTime = Time.time;
                _swipeEnded = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _secondPressPos = Input.mousePosition;
                return true;
            }
            else
            {
                if (Instance._triggerSwipeAtMinLength)
                {
                    return true;
                }
            }

            return false;
        }
        private static bool IsPointerOverUiObject()
        {
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
        private static bool IsDirection(Vector2 direction, Vector2 cardinalDirection)
        {
            var angle = Instance._useEightDirections ? EIGHT_DIR_ANGLE : FOUR_DIR_ANGLE;
            return Vector2.Dot(direction, cardinalDirection) > angle;
        }

        private static Swipe GetSwipeDirByTouch(Vector2 currentSwipe)
        {
            currentSwipe.Normalize();
            var swipeDir = DirectionVectors.FirstOrDefault(dir => IsDirection(currentSwipe, dir.Value));
            return swipeDir.Key;
        }

    }
}