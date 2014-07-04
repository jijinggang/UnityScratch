using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace UnityScratch.core
{
    using ActionTree = BinaryTree<BaseAction>;

    public class ActionMgr
    {
        public List<ActionTree> _roots = new List<ActionTree>();
        public Vector2 CurrStartPos { get; set; }
        public ActionMgr() { 
        }

        public ActionTree Traverse(Func<ActionTree, int, int, object, ActionTree> func, object param)
        {
            foreach(ActionTree node in _roots)
            {
                CurrStartPos = new Vector2(node.Data.x, node.Data.y);
                ActionTree result = node.Traverse(func, 0, 0, param);
                if (result != null)
                    return result;
            }
            return null;
        }
        public void Unlink(ActionTree node){
            if(_roots.Contains(node))
                return;
            node.Unlink();
            _roots.Add(node);
        }
        private static Dictionary<string, BaseAction> _dictAction = new Dictionary<string, BaseAction>();
        public static void Register(BaseAction actionTemplate)
        {
            string id = actionTemplate.Id();
            BaseAction action;
            if (!_dictAction.TryGetValue(id, out action))
            {
                _dictAction[id] = actionTemplate;
            }
        }
        public void AddAction(BaseAction action){
            _roots.Add(new ActionTree(action));
        }
        public void RemoveNode(ActionTree node)
        {
            _roots.Remove(node);
            node.Unlink();
        }
        public static BaseAction CreateAction(string id)
        {
            BaseAction action;
            if (_dictAction.TryGetValue(id, out action))
            {
                return action.Create();
            }
            else
                return null;
        }
    }
}
