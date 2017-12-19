using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Scripts.Attributes;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartHub.UWP.Plugins.Scripts
{
    public static class ScriptEventHelper
    {
        public static void RaiseScriptEvent<TPlugin>(this TPlugin plugin, Expression<Func<TPlugin, ScriptEventHandlerDelegate[]>> expression, params object[] parameters) where TPlugin : PluginBase
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression");

            var eventInfo = memberExpression.Member.GetCustomAttributes<ScriptEventAttribute>().FirstOrDefault();
            if (eventInfo == null)
                throw new InvalidOperationException("Event parameters not found");

            var actions = expression.Compile()(plugin);

            plugin.Run(actions, action => action(eventInfo.EventAlias, parameters));
        }
    }
}
