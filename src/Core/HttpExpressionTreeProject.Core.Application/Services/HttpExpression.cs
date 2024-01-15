using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HttpExpressionTreeProject.Core.Application.Services.HttpExpression;

namespace HttpExpressionTreeProject.Core.Application.Services
{
    public class HttpExpression
    {
        private static readonly Lazy<HttpExpression> httpExpression = new Lazy<HttpExpression>(() =>
        {
            return new HttpExpression();
        });

        public static HttpExpression Instance => HttpExpression.httpExpression.Value;
        protected HttpExpression()
        {

        }


        /// <summary>
        /// Değer karşılaştırma tipi
        /// </summary>
        public enum HttpExperssionRelationalOperatorTypeEnum
        {
            /// <summary>
            /// ==
            /// </summary>
            eq = 1,
            /// <summary>
            /// !=
            /// </summary>
            notEq = 2,
            /// <summary>
            /// Greater >
            /// </summary>
            gt = 3,
            /// <summary>
            /// Less <
            /// </summary>
            ls = 4,

        }

        /// <summary>
        /// Koşul karşılaştırma tipi
        /// </summary>
        public enum HttpExperssionLogicalOperatorTypeEnum
        {
            and = 1,
            or = 2,
            start = 3,
            end = 4,
        }

        public class HttpExpressionData
        {
            public HttpExpressionData()
            {

            }
            public HttpExpressionData(string urlFilter)
            {
                this.UrlFilter = urlFilter;
            }

            public HttpExpressionData(string urlFilter, string propertyName, object filterValue, HttpExperssionRelationalOperatorTypeEnum operationValueTypeEnum, HttpExperssionLogicalOperatorTypeEnum operationQueryrypeEnum) : this(urlFilter)
            {
                this.PropertyName = propertyName;
                this.OperationValueType = operationValueTypeEnum;
                this.FilterValue = filterValue;
                this.ExperssionMergeOperationType = operationQueryrypeEnum;
            }

            public string UrlFilter { get; set; }
            public string PropertyName { get; set; }
            public object FilterValue { get; set; }
            public HttpExperssionRelationalOperatorTypeEnum OperationValueType { get; set; }
            public HttpExperssionLogicalOperatorTypeEnum ExperssionMergeOperationType { get; set; }
        }

        private string Entity { get; set; }
        private string Filter { get; set; }
        public Type EntityType { get; set; }
        public ParameterExpression ParameterExpression { get; set; }



        /// <summary>
        /// Url de iletilen sorgular alındı
        /// </summary>
        /// <param name="entity">ORM Entity</param>
        /// <param name="filter">Experssion sorgu</param>
        /// <returns></returns>
        public HttpExpression SetUrl(string filter)
        {
            this.Filter = filter;
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("$filter veya $entity bulunamadı,HttpException filter formatında bir problem var lüntfen kontrol ediniz ! Örnek : $enttiy=ExampleEnttiyName&$filter=Title=Hello ");
            return HttpExpression.Instance;
        }


        public Expression<Func<T, bool>> GetFilterExperssion<T>()
        {

            this.EntityType = typeof(T);
            this.Entity = this.EntityType.Name;
            this.ParameterExpression = Expression.Parameter(this.EntityType, "x");

            IGroupExpressionComposit baseGroupExpression = ExperssionAbstractFactory.CreateGroupExperssionComposit();
            baseGroupExpression.HttpExpressionData = ExperssionAbstractFactory.CreateHttpExperssionData(this.Filter);


            this.UrlParseToCompositObject(this.Filter, null, baseGroupExpression);

            BinaryExpression binary = baseGroupExpression.GetBuildBinaryExpression();
            Expression<Func<T, bool>> w = Expression.Lambda<Func<T, bool>>(binary, ParameterExpression);
            return w;
        }

        /// <summary>
        /// Belitilen T içerisinde iletilen özellik adını kontrol eder,
        /// özellik eşleştiği durumda true eşleşmediği durumda false döner
        /// <br></br>
        /// Controls the property name passed in the specified T,
        /// returns true if the property matches, false if it does not match
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private bool IsEntityProperty(string propertyName)
        {
            return this.EntityType.GetProperties().Any(x => x.Name.ToLower() == propertyName.Trim().ToLower());
        }

        /// <summary>
        /// IExpressionComposit nesnesi içerisinde HttpExpressionData.UrlFilter string ifadeyi inceler,
        /// string ifade içerisinde ilişkilsel operatörün belirttiği value değere ulaşır.
        /// <br></br>
        /// In the IExpressionComposit object, HttpExpressionData.UrlFilter examines the string expression,
        /// tries to reach the value specified by equality in the string expression.
        /// </summary>
        /// <param name="expressionComposit"></param>
        /// <exception cref="Exception"></exception>
        private void FilterParseValue(IExpressionComposit expressionComposit)
        {
            string newUrl = expressionComposit.HttpExpressionData.UrlFilter.Replace(" ", "");

            int c = expressionComposit.HttpExpressionData.OperationValueType switch
            {
                HttpExperssionRelationalOperatorTypeEnum.notEq => 3,
                HttpExperssionRelationalOperatorTypeEnum.eq => 2,
                HttpExperssionRelationalOperatorTypeEnum.gt => 1,
                _ => 1
            };

            int prop = (expressionComposit.HttpExpressionData.PropertyName).Length + c;
            string value = newUrl.Remove(0, prop);

            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"$filter ifadesi içerisinde value değerine ulaşılamadı !");
            }

            expressionComposit.HttpExpressionData.FilterValue = this.ParseStringToType(value);

        }

        /// <summary>
        /// IExpressionComposit nesnesi içerisinde HttpExpressionData.UrlFilter incelenir, ifade içerisinde 
        /// özellik adı ve ilişkisel operatör tespit edilir.
        /// </summary>
        /// <param name="expressionComposit"></param>
        /// <exception cref="Exception"></exception>
        private void FilterParsePropertyNameAndOperationType(IExpressionComposit expressionComposit)
        {
            string key = string.Empty;
            string filter = expressionComposit.HttpExpressionData.UrlFilter;
            this.EntityType.GetProperties().ToList().ForEach(x =>
            {
                if (filter.StartsWith(x.Name))
                {
                    key = filter.Substring(x.Name.Length + 1, 3).Trim().Split(' ')[0];
                    expressionComposit.HttpExpressionData.PropertyName = x.Name;
                    if (!this.IsEntityProperty(x.Name))
                    {
                        throw new Exception($"{this.EntityType.FullName} içerisinde {x.Name} bulunbamadı !");
                    }
                    return;
                }

            });

            if (string.IsNullOrEmpty(expressionComposit.HttpExpressionData.PropertyName))
            {
                throw new Exception($"{this.EntityType.Name} içerisinde bulunamayan bir PropertyName kullanıldı !");
            }


            if (key == "!eq")
            {
                expressionComposit.HttpExpressionData.OperationValueType = HttpExperssionRelationalOperatorTypeEnum.notEq;
            }
            else if (key == ">")
            {
                expressionComposit.HttpExpressionData.OperationValueType = HttpExperssionRelationalOperatorTypeEnum.gt;
            }
            else if (key == "eq")
            {
                expressionComposit.HttpExpressionData.OperationValueType = HttpExperssionRelationalOperatorTypeEnum.eq;
            }
            else if (key == "<")
            {
                expressionComposit.HttpExpressionData.OperationValueType = HttpExperssionRelationalOperatorTypeEnum.ls;
            }
        }

        /// <summary>
        /// BaseFilter içerisinde filter başlangıç noktası aranır, başlangıç noktası öncesi sorgu hangi mantıksal operatör(or/and) ile eşleştirildiği tespit edilir
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="baseFilter"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private HttpExperssionLogicalOperatorTypeEnum GetQueriyLogicalOperatorType(string filter, string baseFilter, bool group = false)
        {
            if (string.IsNullOrEmpty(baseFilter))
            {
                return HttpExperssionLogicalOperatorTypeEnum.start;
            }

            int index = baseFilter.IndexOf(filter);
            if (index == -1)
            {
                throw new Exception($"{filter} ana sorguya hangi koşul(or/and ) ile birleştirilidiği bulunamadı !");
            }

            if (index == 0)
            {
                return HttpExperssionLogicalOperatorTypeEnum.start;
            }


            string key = baseFilter.Substring(index - (group ? 6 : 4), 4).Trim();
            if (key == "or")
            {
                return HttpExperssionLogicalOperatorTypeEnum.or;
            }
            else if (key == "and")
            {
                return HttpExperssionLogicalOperatorTypeEnum.and;
            }

            throw new Exception($"Operator bulunamadı !");

        }


        private void HttpExpressionSetData(IExpressionComposit expressionComposit, string baseFilterQuery)
        {
            this.FilterParsePropertyNameAndOperationType(expressionComposit);
            this.FilterParseValue(expressionComposit);

            expressionComposit.HttpExpressionData.ExperssionMergeOperationType = this.GetQueriyLogicalOperatorType(expressionComposit.HttpExpressionData.UrlFilter, baseFilterQuery);
            this.HttpExpressionDataCreateBinary(expressionComposit);
        }

        /// <summary>
        /// !!! Recursive Method !!!
        /// <br></br>
        /// Url formatını çözümlemek için kendini tekrarlayan method.
        /// <br></br>
        /// 1. Url üzerinden iletilen Filter yapısını inceler ve parçalar
        /// 2. Parçalara arılan sorguların bir bütünü oluşturacak şekilde gruplanır ve composit patterni uygulanan tek bir obje haline çevirir.
        /// </summary>
        /// <param name="filterQuery"></param>
        /// <param name="baseFilterQuery"></param>
        /// <param name="baseGroupExpressionComposit"></param>
        private void UrlParseToCompositObject(string filterQuery, string baseFilterQuery = null, IGroupExpressionComposit baseGroupExpressionComposit = null)
        {
            string[] filterQuerySplitResult = this.StringOrAndSplit(filterQuery);

            //string ifade içinde or veya and bulunmuyorsa tek sorgu
            if (filterQuerySplitResult.Length <= 1 && filterQuery == filterQuerySplitResult.FirstOrDefault())
            {
                HttpExpressionData expressionData = ExperssionAbstractFactory.CreateHttpExperssionData(filterQuery);

                IExpressionComposit expressionComposit = ExperssionAbstractFactory.CreateExperssionComposit(expressionData);
                HttpExpressionSetData(expressionComposit, baseFilterQuery);
                baseGroupExpressionComposit.AddExpressionComposit(expressionComposit);

                return;
            }

            foreach (string filterItem in filterQuerySplitResult)
            {
                string instanceUrl = filterItem;
                //string ifade parantez içerisinde olduğu durumda parantezleri temizle
                if (filterItem.StartsWith('(') && filterItem.EndsWith(')'))
                {
                    instanceUrl = filterItem.Substring(1, filterItem.Length - 2);
                }
                string[] filterControl = this.StringOrAndSplit(instanceUrl);

                IGroupExpressionComposit instance = baseGroupExpressionComposit;

                //string ifade içinde or veya and operatörleri varsa kendi içinde alt sorgular barındırabilir
                if (!(filterControl.Length <= 1 && instanceUrl == filterControl[0]))
                {
                    HttpExperssionLogicalOperatorTypeEnum groupOperation = this.GetQueriyLogicalOperatorType(instanceUrl, filterQuery, true);
                    HttpExpressionData expressionData = ExperssionAbstractFactory.CreateHttpExperssionData(instanceUrl);
                    IGroupExpressionComposit groupExpressionComposit = ExperssionAbstractFactory.CreateGroupExperssionComposit(expressionData);
                    groupExpressionComposit.HttpExpressionData.ExperssionMergeOperationType = groupOperation;
                    instance.AddExpressionComposit(groupExpressionComposit);
                    instance = groupExpressionComposit;
                }

                this.UrlParseToCompositObject(instanceUrl, filterQuery, instance);
            }

        }

        /// <summary>
        /// HttpExpressionData içerisindeki özellikleri dikkate alarak Expression sorgusu oluşturur
        /// </summary>
        /// <param name="httpExpressionData"></param>
        private void HttpExpressionDataCreateBinary(IExpressionComposit expressionComposit)
        {
            //linq ifadesinin sorgu propertysi .where(EntityName=>EntityName.Title)
            MemberExpression property = Expression.Property(this.ParameterExpression, expressionComposit.HttpExpressionData.PropertyName);

            //sorguya value değeri ekler, .where(EntityName => EntityName.PropertyName == object)
            ConstantExpression constant = Expression.Constant(expressionComposit.HttpExpressionData.FilterValue);

            if (expressionComposit.HttpExpressionData.OperationValueType == HttpExperssionRelationalOperatorTypeEnum.eq)
            {
                expressionComposit.BinaryExpression = Expression.Equal(property, constant);
            }
            else if (expressionComposit.HttpExpressionData.OperationValueType == HttpExperssionRelationalOperatorTypeEnum.notEq)
            {
                expressionComposit.BinaryExpression = Expression.NotEqual(property, constant);

            }
            else if (expressionComposit.HttpExpressionData.OperationValueType == HttpExperssionRelationalOperatorTypeEnum.gt)
            {
                expressionComposit.BinaryExpression = Expression.GreaterThan(property, constant);
            }
            else if (expressionComposit.HttpExpressionData.OperationValueType == HttpExperssionRelationalOperatorTypeEnum.ls)
            {
                expressionComposit.BinaryExpression = Expression.LessThan(property, constant);
            }

        }



        #region Helpers

        /// <summary>
        /// string değeri dönüştürülebilecek en doğru object type çevirir
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        private object ParseStringToType(string stringValue)
        {
            if (stringValue.StartsWith('\'') && stringValue.EndsWith('\''))
            {
                stringValue = stringValue.Substring(1, stringValue.Length - 2);
            }


            // String değerini uygun türe dönüştürme
            if (Guid.TryParse(stringValue, out Guid guidValue))
            {
                return guidValue;
            }
            else if (byte.TryParse(stringValue, out byte byteValue))
            {
                return byteValue;
            }
            else if (int.TryParse(stringValue, out int intValue))
            {
                return intValue;
            }
            else if (bool.TryParse(stringValue, out bool boolValue))
            {
                return boolValue;
            }
            else if (DateTime.TryParse(stringValue, out DateTime dataTimeValue))
            {
                return dataTimeValue;
            }
            else if (stringValue == "null")
            {
                return null;
            }


            // Hiçbir uygun tür bulunamadıysa, string olarak bırak
            return stringValue;
        }

        /// <summary>
        /// filter ifadesi içerisinde string ifadeyi (and|or) key değerlerini dikkate alarak split işlemi uygular
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string[] StringOrAndSplit(string text)
        {
            string pattern = @"(?<!\([^)]*)\s*\b(?:and|or)\b\s*(?![^(]*\))";
            Regex regex = new Regex(pattern);

            return Regex.Split(text, pattern);
        }

        public static BinaryExpression MergeBinary(BinaryExpression left, BinaryExpression right, HttpExperssionLogicalOperatorTypeEnum operationQueryType)
        {
            BinaryExpression merge = null;
            if (operationQueryType == HttpExperssionLogicalOperatorTypeEnum.start)
            {
                return right;
            }
            else if (operationQueryType == null)
            {
                return merge;
            }
            else if (operationQueryType == HttpExperssionLogicalOperatorTypeEnum.and)
            {
                merge = Expression.AndAlso(left, right);
            }
            else if (operationQueryType == HttpExperssionLogicalOperatorTypeEnum.or)
            {
                merge = Expression.OrElse(left, right);
            }

            return merge;
        }

        #endregion End Helpers


    }

    #region ExpressionComposit
    public interface IExpressionComposit
    {
        public BinaryExpression BinaryExpression { get; set; }

        public HttpExpressionData HttpExpressionData { get; set; }
        public BinaryExpression GetBuildBinaryExpression();
    }

    public interface IGroupExpressionComposit : IExpressionComposit
    {

        public List<IExpressionComposit> ExpressionComposits { get; set; }
        public void AddExpressionComposit(IExpressionComposit expressionComposit);

    }


    public class ExpressionComposit : IExpressionComposit
    {
        public BinaryExpression BinaryExpression { get; set; }
        public HttpExpressionData HttpExpressionData { get; set; }

        public ExpressionComposit()
        {

        }

        public ExpressionComposit(HttpExpressionData httpExpressionData)
        {
            HttpExpressionData = httpExpressionData;
        }


        public BinaryExpression GetBuildBinaryExpression()
        {
            return this.BinaryExpression;
        }



    }

    public class GroupExpressionComposit : ExpressionComposit, IGroupExpressionComposit
    {
        public List<IExpressionComposit> ExpressionComposits { get; set; }

        public GroupExpressionComposit()
        {

        }

        public GroupExpressionComposit(HttpExpressionData httpExpressionData) : base(httpExpressionData)
        {
        }


        public void AddExpressionComposit(IExpressionComposit expressionComposit)
        {
            if (this.ExpressionComposits == null)
            {
                this.ExpressionComposits = new List<IExpressionComposit>();
            }
            this.ExpressionComposits.Add(expressionComposit);
        }

        public BinaryExpression GetBuildBinaryExpression()
        {
            BinaryExpression merge = null;
            this.ExpressionComposits?.ToList()?.ForEach(x =>
            {
                BinaryExpression binry = x.GetBuildBinaryExpression();
                if (binry != null)
                {
                    var sss = this;
                    if (x is IGroupExpressionComposit group)
                    {
                        merge = HttpExpression.MergeBinary(merge, binry, group.HttpExpressionData.ExperssionMergeOperationType);
                    }
                    else
                    {
                        merge = HttpExpression.MergeBinary(merge, binry, x.HttpExpressionData.ExperssionMergeOperationType);
                    }

                }


            });

            this.BinaryExpression = merge;
            return merge;

        }




    }
    #endregion End ExpressionComposit

    #region Factory
    public static class ExperssionAbstractFactory
    {
        #region IExpressionComposit
        public static IExpressionComposit CreateExperssionComposit()
        {
            return new ExpressionComposit();
        }

        public static IExpressionComposit CreateExperssionComposit(HttpExpressionData httpExpressionData)
        {
            return new ExpressionComposit(httpExpressionData);
        }


        #endregion End IExpressionComposit

        #region IGroupExpressionComposit
        public static IGroupExpressionComposit CreateGroupExperssionComposit()
        {
            return new GroupExpressionComposit();
        }


        public static IGroupExpressionComposit CreateGroupExperssionComposit(HttpExpressionData httpExpressionData)
        {
            GroupExpressionComposit groupExperssionComposit = new GroupExpressionComposit(httpExpressionData);
            return groupExperssionComposit;
        }
        #endregion End IGroupExpressionComposit

        #region HttpExpressionData
        public static HttpExpressionData CreateHttpExperssionData()
        {
            return new HttpExpressionData();
        }

        public static HttpExpressionData CreateHttpExperssionData(string urlFilter)
        {
            return new HttpExpressionData(urlFilter);
        }

        public static HttpExpressionData CreateHttpExperssionData(string urlFilter, string propertyName, object filterValue, HttpExperssionRelationalOperatorTypeEnum operationValueTypeEnum, HttpExperssionLogicalOperatorTypeEnum operationQueryrypeEnum)
        {
            return new HttpExpressionData(urlFilter, propertyName, filterValue, operationValueTypeEnum, operationQueryrypeEnum);
        }
        #endregion End HttpExpressionData

    }
    #endregion Factory End

    #region Expersion Method
    public static class HttpExpressionExtension
    {
        public static HttpExpression HttExperssion(this HttpContext context)
        {
            string filter = context.Request.Query["$filter"];
            return HttpExpression.Instance.SetUrl(filter);
        }
    }
    #endregion Expersion Method End
}
