using System;
using System.Reflection;

namespace HttpServerLib
{
    public class RouteInfo
    {
        private string controller;
        private string action;
        private string area;
        private Type controllerType;
        private object controllerInstance;

        public string Controller
        {
            get { return controller; }
            set { controller = value; }
        }

        public string Action
        {
            get { return action; }
            set { action = value; UpdateDelegate(); }
        }

        public string Area
        {
            get { return area; }
            set { area = value; UpdateDelegate(); }
        }

        public Type ControllerType
        {
            get { return controllerType; }
            set { controllerType = value; UpdateDelegate(); }
        }

        public object ControllerInstance
        {
            get { return controllerInstance; }
            set { controllerInstance = value; ControllerType = controllerInstance.GetType(); }
        }

        public string BaseRoot { get; set; }

        private Func<object[], object> actionInvoker;

        public Func<object[], object> ActionInvoker => actionInvoker;

        private MethodInfo actionMethodInfo;

        public MethodInfo ActionMethodInfo => actionMethodInfo;

        void UpdateDelegate()
        {
            if (!String.IsNullOrEmpty(Controller))
            {
                //var a1 = AppDomain.CurrentDomain.GetReferencingAssemblies("TestController");
            }

            if (ControllerType != null && !String.IsNullOrEmpty(Action))
            {
                actionMethodInfo = ControllerType.GetMethod(Action);
                actionInvoker = Helper.CreateMethodInvokerDelegate(actionMethodInfo);
            }
        }
    }
}