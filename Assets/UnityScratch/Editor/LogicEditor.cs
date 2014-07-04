using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityScratch.core;
using ActionTree = UnityScratch.core.BinaryTree<UnityScratch.core.BaseAction>;
public class ScratchEditor : EditorWindow
{

    private ActionMgr _mgr = null;
    [MenuItem("UnityScratch/Show Editor Window")]
    static void Init()
    {
        ScratchEditor wnd = EditorWindow.GetWindow<ScratchEditor>();
        wnd.wantsMouseMove = true;
    }
    public void Refresh(ActionMgr mgr)
    {
        _mgr = mgr;
    }
    public ScratchEditor()
    {
        regAllActions();
        initMenu();
    }
    private GameObject _gameobject = null;
    void OnSelectionChange()
    {

        if (_selectedNode != null && Selection.activeObject == _selectedNode.Data)
            return;

        _gameobject = Selection.activeGameObject;
        var script = _gameobject.GetComponent<ScratchScript>();
        if (script != null)
        {
            _mgr = script.mgr;
        }
        else
        {
            _mgr = null;
        }

    }
    void OnGUI()
    {
        if (!checkCondition())
            return;
        if (_mgr == null)
            return;
        onMouseEvent();
        _mgr.Traverse(drawAction, null);
    }
    private bool checkCondition()
    {
        if (_gameobject == null)
        {
            GUILayout.Label("请选择GameObject");
                return false;
        }
        if (_mgr == null)
        {
            GUILayout.Label("请右键向当前选定GameObject添加ScratchScript脚本");
            Event evt = Event.current;
            if (evt.button == 1 && evt.type == EventType.MouseUp)
                showAddScriptMenu();
            return false;
        }
        return true;
    }
    const int W = 200;
    const int H = 20;
    private ActionTree drawAction(ActionTree action, int depth, int offset, object param)
    {
        Vector2 pos = _mgr.CurrStartPos;
        var rect = new Rect(pos.x + depth * H, pos.y + offset * H, W - 20 * depth, H);
        GUI.Box(rect, action.Data.ToString());

        return null;
    }
    private ActionTree GetByPos(Vector2 pos, out bool isLeft)
    {
        bool bLeft = true;
        var result = _mgr.Traverse((ActionTree node, int depth, int offset, object param) =>
        {
            Vector2 pos1 = _mgr.CurrStartPos;
            var rect = new Rect(pos1.x + depth * H, pos1.y + offset * H, W , H);
            if (rect.Contains((Vector2)param))
            {
                bLeft = pos.x < rect.x + rect.width / 2;
                return node;
            }
            return null;
        }, pos);

        isLeft = bLeft;
        return result;

    }

    private void resetNodePos(ActionTree node)
    {
        Vector2 vct = new Vector2(node.Data.x, node.Data.y);

        _mgr.Traverse((curr, depth, offset, param) =>
        {
            if (node == curr)
            {
                Vector2 pos = _mgr.CurrStartPos;
                vct.x = pos.x + depth * H;
                vct.y = pos.y + offset * H;
                return curr;
            }
            return null;
        }, null);
        node.Data.x = vct.x;
        node.Data.y = vct.y;
    }

    private void onMouseEvent()
    {
        Event evt = Event.current;
        //Debug.Log(evt.type);
        if (evt.type == EventType.MouseDown)
        {
            if (evt.button == 1)
            {
                ShowMenu(evt.mousePosition);
            }
            else if (evt.button == 0)
            {
                OnMouseDown(evt);
            }
        }
        else if (evt.type == EventType.MouseUp)
        {
            if (evt.button == 0)
            {
                OnMouseUp(evt);
            }
        }
        else if (evt.type == EventType.MouseDrag || evt.type == EventType.MouseMove)
        {
            OnMouseMove(evt);
        }
    }


    private ActionTree _selectedNode = null;
    //private ActionTree _mouseMovedNode = null;
    private bool _isMoving = false;
    private Vector2 _currMousePos = new Vector2();
    private void OnMouseMove(Event evt)
    {
        _currMousePos = evt.mousePosition;
        if (null == _selectedNode)
            return;
        if (!_isMoving)
        {
            _isMoving = true;
            resetNodePos(_selectedNode);
            _mgr.Unlink(_selectedNode);
        }
        _selectedNode.Data.x += evt.delta.x;
        _selectedNode.Data.y += evt.delta.y;
        Repaint();
    }
    private void OnMouseDown(Event evt)
    {
        bool isLeft = true;
        _selectedNode = GetByPos(evt.mousePosition, out isLeft);
        if (null == _selectedNode)
            return;

        Selection.activeObject = _selectedNode.Data;
        //Editor.CreateEditor(_selectedNode.Data);
        //logNode(_selectedNode, "sel:");
        _isMoving = false;
    }
    private void logNode(ActionTree node, string msg)
    {
        if (node != null)
            Debug.Log(msg + node.Data.ToString());
    }
    private void OnMouseUp(Event evt)
    {
        if (null == _selectedNode)
            return;
        bool isLeft = true;
        var toNode = GetByPos(new Vector2(_selectedNode.Data.x, _selectedNode.Data.y - 1), out isLeft);
        //logNode(toNode, "to:");
        if (toNode != null && toNode != _selectedNode)
        {
            _mgr.RemoveNode(_selectedNode);
            if (isLeft)
                toNode.AddNext(_selectedNode);
            else
                toNode.AddChild(_selectedNode);
        }
        _selectedNode = null;
        Repaint();
    }

    private GenericMenu _menu;
    private GenericMenu _menuAddScript;
    private void initMenu()
    {
        _menu = new GenericMenu();

        foreach (BaseAction action in _allActions)
        {
            BaseAction curr = action;
            _menu.AddItem(new GUIContent(curr.ToString()), false, () =>
            {
                newAction(curr.Id());
            });
        }

        _menuAddScript = new GenericMenu();
        _menuAddScript.AddItem(new GUIContent("Add Scratch Script"), false, () =>
        {
            var script = _gameobject.AddComponent<ScratchScript>();
            _mgr = script.mgr;
        });
    }
    private void showAddScriptMenu()
    {
        _menuAddScript.ShowAsContext();
    }
    private void newAction(string id)
    {
        BaseAction action = ActionMgr.CreateAction(id);
        action.x = _currMousePos.x;
        action.y = _currMousePos.y;
        _mgr.AddAction(action);

    }

    private void ShowMenu(Vector2 pos)
    {
        _menu.ShowAsContext();
    }

    private List<BaseAction> _allActions = new List<BaseAction>(); 
    private void regAllActions()
    {
        _allActions.Add(new CmdLoop());
        _allActions.Add(new CmdMove());
        _allActions.Add(new CmdMoveDiff());
        foreach (BaseAction action in _allActions)
        {
            ActionMgr.Register(action);
        }
    }
}
