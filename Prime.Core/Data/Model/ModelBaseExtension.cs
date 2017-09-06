using System;
using System.Linq.Expressions;
using LiteDB;

namespace Prime.Core
{
    public static class ModelBaseExtension
    {
        public static OpResult SavePublic<T>(this T doc) where T : IModelBase
        {
            return OpResult.From(PublicContext.I.GetDb().Upsert(doc));
        }

        public static OpResult DeletePublic<T>(this T doc) where T : IModelBase
        {
            return OpResult.From(PublicContext.I.GetDb().Delete<T>(doc.Id));
        }

        public static OpResult Save<T>(this T doc, IDataContext context) where T : IModelBase
        {
            return OpResult.From(context.GetDb().Upsert(doc));
        }

        public static OpResult Delete<T>(this T doc, IDataContext context) where T : IModelBase
        {
            return OpResult.From(context.GetDb().Delete<T>(doc.Id));
        }

        public static T FirstOrDefault<T>(this LiteQueryable<T> query,  Expression<Func<T, bool>> predicate)
        {
            return query.Where(predicate).FirstOrDefault();
        }
    }
}