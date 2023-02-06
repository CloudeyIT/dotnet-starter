using DotnetStarter.Core.Framework.Database;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace DotnetStarter.Core.Framework.Identity.Extensions;

public static class PolicyExtensions
{
    /// <summary>
    /// Require the parent object to fulfil the given assertion.
    /// If the policy is applied to a field, the parent is the class that contains the field.
    /// If the policy is applied to a class, the parent is the class itself.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T">Type of the parent</typeparam>
    /// <returns></returns>
    public static AuthorizationPolicyBuilder RequireParentAssertion<T> (
        this AuthorizationPolicyBuilder builder,
        Func<T, AuthorizationHandlerContext, IResolverContext, bool> expression
    )
    {
        return builder.RequireAssertion(
            context =>
            {
                switch (context.Resource)
                {
                    case IResolverContext resolverContext:
                        {
                            var parent = resolverContext.Parent<T>();
                            return expression.Invoke(parent, context, resolverContext);
                        }
                    default:
                        throw new ApplicationException(
                            "Invalid target for parent assertion requirement. Must be applied to a class or field in a GraphQL context!"
                        );
                }
            }
        );
    }

    // /// <summary>
    // /// Require the parent object to fulfil the given assertion using the database.
    // /// If the policy is applied to a field, the parent is the class that contains the field.
    // /// If the policy is applied to a class, the parent is the class itself.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <typeparam name="T">Type of the parent</typeparam>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireParentAssertion<T> (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<T, AuthorizationHandlerContext, MainDb, IResolverContext, bool> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case T:
    //                     throw new ApplicationException(
    //                         "Invalid context for parent assertion requirement. Database cannot be resolved outside GraphQL context!"
    //                     );
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var db = resolverContext.Service<MainDb>();
    //                         var parent = resolverContext.Parent<T>();
    //                         return expression.Invoke(parent, context, db, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for parent assertion requirement. Must be applied to a class or field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the parent object to fulfil the given assertion.
    // /// If the policy is applied to a field, the parent is the class that contains the field.
    // /// If the policy is applied to a class, the parent is the class itself.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <typeparam name="T">Type of the parent</typeparam>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireParentAssertion<T> (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<T, AuthorizationHandlerContext, IResolverContext, Task<bool>> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         async context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var parent = resolverContext.Parent<T>();
    //                         return await expression.Invoke(parent, context, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for parent assertion requirement. Must be applied to a class or field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the parent object to fulfil the given assertion using the database.
    // /// If the policy is applied to a field, the parent is the class that contains the field.
    // /// If the policy is applied to a class, the parent is the class itself.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <typeparam name="T">Type of the parent</typeparam>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireParentAssertion<T> (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<T, AuthorizationHandlerContext, MainDb, IResolverContext, Task<bool>> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         async context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case T:
    //                     throw new ApplicationException(
    //                         "Invalid context for parent assertion requirement. Database cannot be resolved outside GraphQL context!"
    //                     );
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var db = resolverContext.Service<MainDb>();
    //                         var parent = resolverContext.Parent<T>();
    //                         return await expression.Invoke(parent, context, db, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for parent assertion requirement. Must be applied to a class or field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the field to fulfil the given assertion.
    // /// This policy MUST be applied to a field, NOT a class.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <typeparam name="T">Type of the field</typeparam>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireFieldAssertion<T> (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<T, AuthorizationHandlerContext, IResolverContext, bool> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         async context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var field = await resolverContext.ResolveAsync<T>();
    //                         return expression.Invoke(field, context, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for field assertion requirement. Must be applied to a field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the field to fulfil the given assertion using the database.
    // /// This policy MUST be applied to a field, NOT a class.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <typeparam name="T">Type of the field</typeparam>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireFieldAssertion<T> (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<T, AuthorizationHandlerContext, MainDb, IResolverContext, bool> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         async context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var db = resolverContext.Service<MainDb>();
    //                         var field = await resolverContext.ResolveAsync<T>();
    //                         return expression.Invoke(field, context, db, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for field assertion requirement. Must be applied to a field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the field to fulfil the given assertion.
    // /// This policy MUST be applied to a field, NOT a class.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <typeparam name="T">Type of the field</typeparam>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireFieldAssertion<T> (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<T, AuthorizationHandlerContext, IResolverContext, Task<bool>> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         async context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var field = await resolverContext.ResolveAsync<T>();
    //                         return await expression.Invoke(field, context, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for field assertion requirement. Must be applied to a field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the field to fulfil the given assertion using the database.
    // /// This policy MUST be applied to a field, NOT a class.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <typeparam name="T">Type of the field</typeparam>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireFieldAssertion<T> (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<T, AuthorizationHandlerContext, MainDb, IResolverContext, Task<bool>> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         async context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var db = resolverContext.Service<MainDb>();
    //                         var field = await resolverContext.ResolveAsync<T>();
    //                         return await expression.Invoke(field, context, db, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for field assertion requirement. Must be applied to a field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the assertion to be true, using the database. NOTE: Consider using the async overload of this method instead.
    // /// WARNING: Using this assertion may cause a large number of database queries, adversely affecting performance.
    // /// This requirement can only be used in GraphQL context.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireDatabaseAssertion (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<MainDb, AuthorizationHandlerContext, IResolverContext, bool> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var db = resolverContext.DbContext<MainDb>();
    //                         return expression.Invoke(db, context, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for field assertion policy. Must be applied to a field in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
    //
    // /// <summary>
    // /// Require the assertion to be true, using the database.
    // /// WARNING: Using this assertion may cause a large number of database queries, adversely affecting performance.
    // /// This requirement can only be used in GraphQL context.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <param name="expression"></param>
    // /// <returns></returns>
    // public static AuthorizationPolicyBuilder RequireDatabaseAssertion (
    //     this AuthorizationPolicyBuilder builder,
    //     Func<MainDb, AuthorizationHandlerContext, IResolverContext, Task<bool>> expression
    // )
    // {
    //     return builder.RequireAssertion(
    //         async context =>
    //         {
    //             switch (context.Resource)
    //             {
    //                 case IResolverContext resolverContext:
    //                     {
    //                         var db = resolverContext.DbContext<MainDb>();
    //                         return await expression.Invoke(db, context, resolverContext);
    //                     }
    //                 default:
    //                     throw new ApplicationException(
    //                         "Invalid target for field assertion requirement. Must be applied in a GraphQL context!"
    //                     );
    //             }
    //         }
    //     );
    // }
}