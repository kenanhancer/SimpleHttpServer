using System;
using System.Reflection;

namespace HttpServerLib
{
    public class RouteInfo
    {
        private Type controllerType;

        public Type ControllerType
        {
            get { return controllerType; }
            set { controllerType = value; UpdateDelegate(); }
        }

        private object controllerInstance;

        public object ControllerInstance
        {
            get { return controllerInstance; }
            set { controllerInstance = value; ControllerType = controllerInstance.GetType(); }
        }


        private string actionName;

        public string ActionName
        {
            get { return actionName; }
            set { actionName = value; UpdateDelegate(); }
        }

        public string BaseRoot { get; set; }

        private Func<object[], object> actionInvoker;

        public Func<object[], object> ActionInvoker => actionInvoker;

        private MethodInfo actionMethodInfo;

        public MethodInfo ActionMethodInfo => actionMethodInfo;

        void UpdateDelegate()
        {
            if(ControllerType!=null && !String.IsNullOrEmpty(ActionName))
            {
                actionMethodInfo = ControllerType.GetMethod(ActionName);
                actionInvoker = Helper.CreateMethodInvokerDelegate(actionMethodInfo);
            }
        }
    }
}