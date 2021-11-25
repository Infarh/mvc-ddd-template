using System.Collections.Generic;
using SolutionTemplate.Interfaces.Base.Repositories;

namespace SolutionTemplate.DAL.Repositories;

internal record Page<T>(IEnumerable<T> Items, int TotalCount, int PageNumber, int PageSize) : IPage<T>;