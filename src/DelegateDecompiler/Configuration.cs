using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime;

namespace DelegateDecompiler
{
    public abstract class Configuration
    {
        private static readonly object locker = new object();
        private static volatile Configuration instance;

        private ConcurrentDictionary<MemberInfo, LambdaExpression> ExpressionMaps { get; set; } = new ConcurrentDictionary<MemberInfo, LambdaExpression>();

        internal static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new DefaultConfiguration();
                        }
                    }
                }
                return instance;
            }
        }

        public static void Configure(Configuration cfg)
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = cfg;
                        return;
                    }
                }
            }

            throw new InvalidOperationException("DelegateDecompiler has been configured already");
        }

        public void AddExpressionMap<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> expression, Expression<Func<TEntity, TProperty>> expressionMap)
            where TEntity : class
        {
            var memberInfo = ExtractMemberInfo(expression);

            RegisterDecompileableMember(memberInfo);

            if (memberInfo is PropertyInfo)
            {
                memberInfo = ((PropertyInfo)memberInfo).GetGetMethod();
            }
            
            ExpressionMaps[memberInfo] = expressionMap;
        }

        public bool TryGetExpressionMap(MemberInfo memberInfo, out LambdaExpression expression)
        {
            if (ExpressionMaps.ContainsKey(memberInfo))
            {
                expression = ExpressionMaps[memberInfo];

                return true;
            }

            expression = null;
            return false;
        }

        public abstract bool ShouldDecompile(MemberInfo memberInfo);

        public virtual void RegisterDecompileableMember(MemberInfo property)
        {
            throw new NotImplementedException("The method RegisterDecompileableMember of Configuration is not implemented. Please implement it in your custom configuration class.");
        }

        private static MemberInfo ExtractMemberInfo(Expression body)
        {
            if (body.NodeType == ExpressionType.MemberAccess)
            {
                var member = ((MemberExpression)body).Member;
                if (!(member is PropertyInfo))
                {
                    throw new InvalidOperationException("MemberExpression expected to have a Member of PropertyInfo type, but got " + member.GetType().Name);
                }
                return member;
            }
            if (body.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression)body).Method;
            }
            if (body.NodeType == ExpressionType.Lambda)
            {
                return ExtractMemberInfo(((LambdaExpression)body).Body);
            }
            throw new ArgumentException("Expression expected to be of MemberAccess or Call type, but got " + body.NodeType);
        }
    }
}