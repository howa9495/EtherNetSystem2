using UnityEngine;
using PlayerSystems;
using UnityEngine.Events;
using System.Reflection;
using System;
using UnityEditor.Rendering;
using NUnit.Framework;
using System.Collections.Generic;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlayerSystems
{
    public enum MappedAction
    {
        move,
        look,
        jump,
        sprint,
        crouch,
        zoom,
        interact,
        item
    }

    public class InputMapper : MonoBehaviour
    {
        [HideInInspector][SerializeField] private MappedAction _action;
        public MappedAction action
        {
            get { return _action; }
            set
            {
                _action = value;
                UnSubEvent();
                FindReference();
                SubEvent();
            }
        }
        private string methodName;

        PlayerInputMapping playerInputMapping;

        [HideInInspector] public UnityEvent<int> intEvent;
        [HideInInspector] public UnityEvent<bool> boolEvent;
        [HideInInspector] public UnityEvent<float> floatEvent;
        [HideInInspector] public UnityEvent<Vector2> vector2Event;
        [HideInInspector] public UnityEvent<float> upEvent;
        [HideInInspector] public UnityEvent<float> downEvent;
        [HideInInspector] public UnityEvent<float> leftEvent;
        [HideInInspector] public UnityEvent<float> rightEvent;

        FieldInfo fieldInfo;
        EventInfo eventInfo;
        EventHandler<object> eventHandler;
        public Type inputType = typeof(bool);

        [HideInInspector][SerializeField] bool _flipflop;
        [HideInInspector]public bool flipflop
        {
            get
            {
                return _flipflop;
            }
            set
            {
                _flipflop = value;
                UnSubEvent();
                FindReference();
                SubEvent();

            }
        }
        [HideInInspector] public bool flipflopSwitch = false;

        [HideInInspector] public bool directionOnly = false;
        List<bool> dirTrigger = new List<bool>() { false, false, false, false };
        [HideInInspector] public bool releaseTrigger = false;



        private object _input;
        public object input
        {
            get { return _input; }
            set
            {
                if (value != _input)
                {
                    if (fieldInfo.GetValue(playerInputMapping) != _input)
                    {
                        _input = value;
                        ActiveEvent(_input);
                    }
                }
            }
        }
        private void OnEnable()
        {

            FindReference();
            SubEvent();
        }
        private void OnDisable()
        {
            UnSubEvent();
        }
        void Awake()
        {
            playerInputMapping = PlayerManager.Instance.playerInputMapping;
            FindReference();
        }

 
        private void FindReference()
        {
            string valueName = action.ToString();
            if (playerInputMapping == null) playerInputMapping = FindFirstObjectByType<PlayerInputMapping>();
            fieldInfo = playerInputMapping.GetType().GetField(valueName);//用變量名尋找變量


            string eventName = (valueName + "Event").ToString();
            eventInfo = playerInputMapping.GetType().GetEvent(eventName);//用事件名尋找事件
            if (eventInfo == null) { Debug.Log("Not found eventInfo."); }

            UpdateValue();
        }

        void UpdateValue()
        {
            eventHandler = (sender, e) =>
            {

                bool? isBool = e as bool?;
                if (isBool != null)
                {
                    bool value = (bool)e;
                    if (flipflop)
                    {
                        flipflopSwitch = value ? !flipflopSwitch : flipflopSwitch;
                    }
                    input = flipflop ? flipflopSwitch : e;
                }
                else
                {
                    flipflop = false;
                }
                Vector2? isVector2 = e as Vector2?;
                if (isVector2 != null)
                {
                    input = (Vector2)e;
                }
                else
                {
                    directionOnly = false; 
                }
                
            };
        }





        void SubEvent()//訂閱事件
        {
            if (eventInfo != null) eventInfo.AddEventHandler(playerInputMapping, eventHandler);
        }
        void UnSubEvent()//解除訂閱事件
        {
            if (eventInfo != null) eventInfo.RemoveEventHandler(playerInputMapping, eventHandler);
        }
        public void ActiveEvent(object o)//分類並相關事件
        {
            bool? isBool = o as bool?;
            if (isBool != null)
            {
                bool value = (bool)o;
                boolEvent?.Invoke(value);
                inputType = typeof(bool);
            }
            if (o.GetType() == typeof(int))
            {
                int value = (int)o;
                intEvent?.Invoke(value);
                inputType = typeof(int);
            }
            if (o.GetType() == typeof(float))
            {
                float value = (float)o;
                floatEvent?.Invoke(value);
                inputType = typeof(float);
            }
            //Vector2? isVector2 = o as Vector2?;
            if (o.GetType() == typeof(Vector2))
            {
                Vector2 value = (Vector2)o;
                if (directionOnly == false)
                {
                    vector2Event?.Invoke(value);
                }
                else
                {
                    List<UnityEvent<float>> eventList = new List<UnityEvent<float>> { upEvent, downEvent, rightEvent, leftEvent };
                    for (int i = 0; i < eventList.Count; i++)
                    {
                        UnityEvent<float> currentEvent = eventList[i];
                        if (currentEvent.GetPersistentEventCount() > 0)
                        {
                            float v = (i <= 1) ? value.y : value.x;
                            bool r = (i + 1) % 2 == 1;
                            bool b = (r && v > 0.5) || (!r && v < -0.5);
                            //Debug.Log("eventList: " + i + " notEmpty: " + (eventList[i].GetPersistentEventCount()) + " y: " + value.y + " x: " + value.x + " odd: " + r + " B: " + b);
                            if (b)
                            {
                                if (!releaseTrigger) eventList[i].Invoke(v);
                            }
                            else
                            {
                                if (releaseTrigger && dirTrigger[i]) eventList[i].Invoke(v);
                            }
                            dirTrigger[i] = b;
                        }
                    }

                    inputType = typeof(Vector2);
                }
            }
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(InputMapper))]
    public class InputMapperEditor : Editor
    {
        InputMapper _inputMapper;
        PlayerInputMapping _playerInputMapping;

        //用作暫存serializedObject的數據並在inspector使用
        SerializedProperty _action, _flipflop, _directionOnly, _releaseTrigger;

        //接收SerializedProperty的數據回饋到InputMapper
        MappedAction actionValue;
        bool flipflopValue;
        bool directionOnly;
        bool releaseTrigger;
        private void OnEnable()
        {
            _inputMapper = (InputMapper)target;
            _playerInputMapping = FindFirstObjectByType<PlayerInputMapping>();

            _action = serializedObject.FindProperty("_action");
            _flipflop = serializedObject.FindProperty("_flipflop");
            _directionOnly = serializedObject.FindProperty("directionOnly");
            _releaseTrigger = serializedObject.FindProperty("releaseTrigger");

            _action.enumValueIndex = (int)_inputMapper.action;
            _flipflop.boolValue = _inputMapper.flipflop;
            _directionOnly.boolValue = _inputMapper.directionOnly;
            _releaseTrigger.boolValue = _inputMapper.releaseTrigger;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            FieldInfo source = _playerInputMapping.GetType().GetField(_inputMapper.action.ToString());


            DrawDefaultInspector();

            //暫存從inspector更改的數據
            _action.SetEnumValue((MappedAction)EditorGUILayout.EnumPopup("Action", (MappedAction)_action.enumValueIndex));

            // 根據InputMapper.action類型動態隱藏不需要的UnityEvent字段
            if (source.FieldType == typeof(bool))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("boolEvent"), new GUIContent("Bool Event"));

                //暫存從inspector更改的數據
                _flipflop.boolValue = (bool)EditorGUILayout.Toggle("Flip Flop", (bool)_flipflop.boolValue);

                //條件適合才會顯示
                if (_inputMapper.flipflop) EditorGUILayout.Toggle("Flip Flop Switch", _inputMapper.flipflopSwitch);

            }

            if (source.FieldType == typeof(int))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("intEvent"), new GUIContent("Int Event"));
            }

            if (source.FieldType == typeof(float))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("floatEvent"), new GUIContent("Float Event"));
            }

            if (source.FieldType == typeof(Vector2))
            {
                _directionOnly.boolValue = (bool)EditorGUILayout.Toggle("Direction Only", (bool)_directionOnly.boolValue);
                if (!_inputMapper.directionOnly)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("vector2Event"), new GUIContent("Vector2 Event"));
                }
                else
                {
                    _releaseTrigger.boolValue = (bool)EditorGUILayout.Toggle("Release Only", (bool)_releaseTrigger.boolValue);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("upEvent"), new GUIContent("Up Event"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("downEvent"), new GUIContent("Down Event"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("rightEvent"), new GUIContent("Right Event"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("leftEvent"), new GUIContent("Left Event"));

                }
            }

            actionValue = (MappedAction)_action.enumValueIndex;
            flipflopValue = _flipflop.boolValue;
            directionOnly = _directionOnly.boolValue;
            releaseTrigger = _releaseTrigger.boolValue;

            _inputMapper.action = actionValue;
            _inputMapper.flipflop = flipflopValue;
            _inputMapper.directionOnly = directionOnly;
            _inputMapper.releaseTrigger = releaseTrigger;

            serializedObject.ApplyModifiedProperties();


        }

    }
#endif
}