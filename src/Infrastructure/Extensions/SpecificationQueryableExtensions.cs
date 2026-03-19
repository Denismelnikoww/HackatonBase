<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 576aa0d (наговнокодил войд не бей)
﻿//using Domain.Specification.Base;
//using Microsoft.EntityFrameworkCore;
//using ResultSharp.Core;
//using ResultSharp.Errors;
<<<<<<< HEAD

//namespace Infrastructure.Extensions
//{
//    public static class SpecificationQueryableExtensions
//    {
//        public static IQueryable<T> Where<T>(this IQueryable<T> query, Specification<T> specification)
//            where T : class
//        {
//            var expression = specification.ToExpression();
//            return query.Where(expression);
//        }

//        public static async Task<Result<T>> FirstOrDefaultAsync<T>(
//             this IQueryable<T> query,
//             Specification<T> specification,
//             CancellationToken cancellationToken = default)
//             where T : class
//        {
//            var defaultError = Error.NotFound($"Entity of type {typeof(T).Name} not found");

//            var entity = await query.Where(specification)
//                .FirstOrDefaultAsync(cancellationToken);

//            if (entity == null)
//            {
//                return defaultError;
//            }
//            return entity;
//        }

//        public static async Task<Result<T>> FirstOrDefaultAsync<T>(
//            this IQueryable<T> query,
//            Specification<T> specification,
//            Error errorOnNotFound,
//            CancellationToken cancellationToken = default)
//            where T : class
//        {
//            var entity = await query.Where(specification)
//                .FirstOrDefaultAsync(cancellationToken);

//            if (entity == null)
//            {
//                return errorOnNotFound;
//            }
//            return entity;
//        }
//    }
//}
=======
﻿using Domain.Specification.Base;
using Microsoft.EntityFrameworkCore;
using ResultSharp.Core;
using ResultSharp.Errors;
=======
>>>>>>> 576aa0d (наговнокодил войд не бей)

//namespace Infrastructure.Extensions
//{
//    public static class SpecificationQueryableExtensions
//    {
//        public static IQueryable<T> Where<T>(this IQueryable<T> query, Specification<T> specification)
//            where T : class
//        {
//            var expression = specification.ToExpression();
//            return query.Where(expression);
//        }

//        public static async Task<Result<T>> FirstOrDefaultAsync<T>(
//             this IQueryable<T> query,
//             Specification<T> specification,
//             CancellationToken cancellationToken = default)
//             where T : class
//        {
//            var defaultError = Error.NotFound($"Entity of type {typeof(T).Name} not found");

//            var entity = await query.Where(specification)
//                .FirstOrDefaultAsync(cancellationToken);

//            if (entity == null)
//            {
//                return defaultError;
//            }
//            return entity;
//        }

//        public static async Task<Result<T>> FirstOrDefaultAsync<T>(
//            this IQueryable<T> query,
//            Specification<T> specification,
//            Error errorOnNotFound,
//            CancellationToken cancellationToken = default)
//            where T : class
//        {
//            var entity = await query.Where(specification)
//                .FirstOrDefaultAsync(cancellationToken);

<<<<<<< HEAD
            if (entity == null)
            {
                return errorOnNotFound;
            }
            return entity;
        }
    }
}
>>>>>>> ae4223f (ща спец добавлять буду)
=======
//            if (entity == null)
//            {
//                return errorOnNotFound;
//            }
//            return entity;
//        }
//    }
//}
>>>>>>> 576aa0d (наговнокодил войд не бей)
