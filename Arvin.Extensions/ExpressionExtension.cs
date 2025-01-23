using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Arvin.Extensions
{
    public static class ExpressionExtension
    {
        public static ExpressionParser ParseExpression<T>(this Expression<Func<T, bool>> expression)
        {
            return new ExpressionParser().Parse(expression);
        }
    }

    public class ExpressionItem
    {
        public ExpressionItem(string propertyName, object value)
        {
            PropertyName = propertyName;
            Value = value;
        }
        public string PropertyName { get; set; }
        public object Value { get; set; }
    }
    /// <summary>
    /// 表达式树解析器
    /// </summary>
    public class ExpressionParser : ExpressionVisitor
    {
        List<ExpressionItem> _resList = new List<ExpressionItem>();
        List<ExpressionType> _orderTypeList = new List<ExpressionType>();
        public string ExpressionString { get; private set; }
        public ExpressionParser Parse<T>(Expression<Func<T, bool>> expression)
        {
            ParseExpression(expression.Body);
            return this;
        }

        private void ParseExpression(Expression expr)
        {
            if (expr is BinaryExpression binaryExpression)
            {
                // 处理逻辑与（&&）
                if (binaryExpression.NodeType == ExpressionType.AndAlso)
                {
                    ParseExpression(binaryExpression.Left);
                    _orderTypeList.Add(binaryExpression.NodeType);
                    ParseExpression(binaryExpression.Right);
                }
                else
                {
                    ParseComparison(binaryExpression);
                }
                // 注意：这里忽略了其他类型的二元表达式，如逻辑或（||）、算术运算等。
            }
            // 注意：这里也忽略了其他类型的表达式，如一元表达式、方法调用表达式等。
        }

        private void ParseComparison(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left is MemberExpression memberExpression &&
                binaryExpression.Right is ConstantExpression constantExpression)
            {
                // 处理成员访问与常量比较
                HandleMemberAndConstant(memberExpression, constantExpression);
            }
            else if (binaryExpression.Right is MethodCallExpression methodCallExpression)
            {
                // 特别处理 DateTime.Now
                HandleMethodCall(binaryExpression.Left, methodCallExpression);
            }
            else
            {
                string propertyName = (binaryExpression.Left as MemberExpression).Member.Name;
                object value = binaryExpression.Right.ToString();

                var res = new ExpressionItem(propertyName, value);
                _resList.Add(res);
            }
            // 注意：这里忽略了其他类型的二元表达式和子表达式。
        }

        private void HandleMemberAndConstant(MemberExpression memberExpression, ConstantExpression constantExpression)
        {
            string propertyName = memberExpression.Member.Name;
            object value = constantExpression.Value;

            var res = new ExpressionItem(propertyName, value);
            _resList.Add(res);
        }

        private void HandleMethodCall(Expression left, MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Method.DeclaringType == typeof(DateTime) &&
                methodCallExpression.Method.Name == "get_Now")
            {
                // 这里我们特别处理了 DateTime.Now，但请注意，我们无法直接获取其运行时值作为常量。
                // 一种选择是记录这个事实，并在后续处理中特别考虑。
                // 另一种选择是替换为一个具体的日期时间值（但这需要在表达式构建之前进行）。

                // 示例：记录属性名和 DateTime.Now 的事实（不记录具体值）
                string propertyName = ((MemberExpression)left).Member.Name;
                var res = new ExpressionItem(propertyName, "DateTime.Now");
                _resList.Add(res);

                // 注意：这里的 "DateTime.Now" 是一个字符串，不是实际的日期时间值。
                // 如果你需要在后续处理中使用这个值，你需要决定如何获取它。
            }
            // 注意：这里忽略了其他类型的方法调用表达式。
        }

        public List<ExpressionItem> GetResult()
        {
            return _resList;
        }


    }
}
