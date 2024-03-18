﻿using System.Linq.Expressions;

namespace CatenaccioStore.Core.Repositories.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        List<Expression<Func<T, object>>> Includes { get;}

    }
}