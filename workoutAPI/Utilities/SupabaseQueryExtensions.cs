using System.Collections.Generic;
using Supabase.Postgrest.Interfaces;
using Supabase.Postgrest.Models;

namespace workoutAPI.Utilities;

public static class SupabaseQueryExtensions
{
    public static IPostgrestTable<T> ApplyFilters<T>(
        this IPostgrestTable<T> query, 
        List<SupabaseQueryBuilder.FilterItem> filters) where T : BaseModel, new()
    {
        foreach (var filter in filters)
        {
            var operatorEnum = MapOperator(filter.Operator);
            query = query.Filter(filter.Field, operatorEnum, filter.Value);
        }
        return query;
    }

    private static Supabase.Postgrest.Constants.Operator MapOperator(string op)
    {
        return op.ToLower() switch
        {
            "eq" => Supabase.Postgrest.Constants.Operator.Equals,
            "neq" => Supabase.Postgrest.Constants.Operator.NotEqual,
            "gt" => Supabase.Postgrest.Constants.Operator.GreaterThan,
            "gte" => Supabase.Postgrest.Constants.Operator.GreaterThanOrEqual,
            "lt" => Supabase.Postgrest.Constants.Operator.LessThan,
            "lte" => Supabase.Postgrest.Constants.Operator.LessThanOrEqual,
            "like" => Supabase.Postgrest.Constants.Operator.Like,
            "ilike" => Supabase.Postgrest.Constants.Operator.ILike,
            "in" => Supabase.Postgrest.Constants.Operator.In,
            _ => Supabase.Postgrest.Constants.Operator.Equals
        };
    }
}