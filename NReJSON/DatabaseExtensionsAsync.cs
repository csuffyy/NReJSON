using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NReJSON
{
    public static partial class DatabaseExtensions
    {
        /// <summary>
        /// `JSON.DEL`
        /// 
        /// Delete a value.
        ///
        /// Non-existing keys and paths are ignored. Deleting an object's root is equivalent to deleting the key from Redis.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsondel
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path">Defaults to root if not provided.</param>
        /// <returns>Integer, specifically the number of paths deleted (0 or 1).</returns>
        public static async Task<int> JsonDeleteAsync(this IDatabase db, RedisKey key, string path = ".") =>
            (int)(await db.ExecuteAsync(GetCommandName(CommandType.Json.DEL), CombineArguments(key, path)));

        /// <summary>
        /// `JSON.GET`
        /// 
        /// Return the value at `path` in JSON serialized form.
        /// 
        /// `NOESCAPE` is `true` by default.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static Task<RedisResult> JsonGetAsync(this IDatabase db, RedisKey key, params string[] paths) =>
            db.JsonGetAsync(key, true, paths);

        /// <summary>
        /// `JSON.GET`
        /// 
        /// Return the value at `path` in JSON serialized form.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="noEscape">This option will disable the sending of \uXXXX escapes for non-ascii characters. This option should be used for efficiency if you deal mainly with such text.</param>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static Task<RedisResult> JsonGetAsync(this IDatabase db, RedisKey key, bool noEscape, params string[] paths) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.GET), CombineArguments(key, noEscape ? "NOESCAPE" : string.Empty, PathsOrDefault(paths, new[] { "." })));

        /// <summary>
        /// `JSON.MGET`
        /// 
        /// Returns the values at `path` from multiple `key`s. Non-existing keys and non-existing paths are reported as null.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonmget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys"></param>
        /// <param name="path"></param>
        /// <returns>Array of Bulk Strings, specifically the JSON serialization of the value at each key's path.</returns>
        public static Task<RedisResult[]> JsonMultiGetAsync(this IDatabase db, string[] keys, string path = ".") =>
            db.JsonMultiGetAsync(keys.Select(k => (RedisKey)k).ToArray(), path);

        /// <summary>
        /// `JSON.MGET`
        /// 
        /// Returns the values at `path` from multiple `key`s. Non-existing keys and non-existing paths are reported as null.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonmget
        /// </summary>
        /// <param name="db"></param>
        /// <param name="keys"></param>
        /// <param name="path"></param>
        /// <returns>Array of Bulk Strings, specifically the JSON serialization of the value at each key's path.</returns>
        public static async Task<RedisResult[]> JsonMultiGetAsync(this IDatabase db, RedisKey[] keys, string path = ".") =>
            (RedisResult[])(await db.ExecuteAsync(GetCommandName(CommandType.Json.MGET), CombineArguments(keys, path)));

        /// <summary>
        /// `JSON.SET`
        /// 
        /// Sets the JSON value at path in key
        ///
        /// For new Redis keys the path must be the root. 
        /// 
        /// For existing keys, when the entire path exists, the value that it contains is replaced with the json value.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonset
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="json"></param>
        /// <param name="path"></param>
        /// <param name="setOption"></param>
        /// <returns></returns>
        public static Task<RedisResult> JsonSetAsync(this IDatabase db, RedisKey key, string json, string path = ".", SetOption setOption = SetOption.Default) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.SET), CombineArguments(key, path, json, GetSetOptionString(setOption)));

        /// <summary>
        /// `JSON.TYPE`
        /// 
        /// Report the type of JSON value at `path`.
        ///
        /// `path` defaults to root if not provided.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsontype
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Task<RedisResult> JsonTypeAsync(this IDatabase db, RedisKey key, string path = ".") =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.TYPE), CombineArguments(key, path));

        /// <summary>
        /// `JSON.NUMINCRBY`
        /// 
        /// Increments the number value stored at `path` by `number`.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonnumincrby
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="number"></param>
        public static Task<RedisResult> JsonIncrementNumberAsync(this IDatabase db, RedisKey key, string path, double number) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.NUMINCRBY), CombineArguments(key, path, number));

        /// <summary>
        /// `JSON.NUMMULTBY`
        /// 
        /// Multiplies the number value stored at `path` by `number`.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonnummultby
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="number"></param>
        public static Task<RedisResult> JsonMultiplyNumberAsync(this IDatabase db, RedisKey key, string path, double number) =>
            db.ExecuteAsync(GetCommandName(CommandType.Json.NUMMULTBY), CombineArguments(key, path, number));

        /// <summary>
        /// [Not implemented yet]
        /// 
        /// `JSON.STRAPPEND`
        /// 
        /// Append the json-string value(s) the string at path.
        ///
        /// path defaults to root if not provided.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonstrappend
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="jsonString"></param>
        /// <returns>Length of the new JSON object.</returns>
        public static Task<int> JsonAppendStringAsync(this IDatabase db, RedisKey key, string path = ".", string jsonString = "{}") =>
            throw new NotImplementedException("This doesn't work, not sure what I'm doing wrong here.");

        /// <summary>
        /// `JSON.STRLEN`
        /// 
        /// Report the length of the JSON String at `path` in `key`.
        ///
        /// `path` defaults to root if not provided. If the `key` or `path` do not exist, null is returned.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonstrlen
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns>Integer, specifically the string's length.</returns>
        public static async Task<int> JsonStringLengthAsync(this IDatabase db, RedisKey key, string path) =>
            (int)(await db.ExecuteAsync(GetCommandName(CommandType.Json.STRLEN), CombineArguments(key, path)));

        /// <summary>
        /// `JSON.ARRAPPEND`
        /// 
        /// Append the `json` value(s) into the array at `path` after the last element in it.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrappend
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="json"></param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static async Task<int> JsonArrayAppendAsync(this IDatabase db, RedisKey key, string path, params string[] json) =>
            (int)(await db.ExecuteAsync(GetCommandName(CommandType.Json.ARRAPPEND), CombineArguments(key, path, json)));

        /// <summary>
        /// `JSON.ARRINDEX`
        /// 
        /// Search for the first occurrence of a scalar JSON value in an array.
        ///
        /// The optional inclusive `start`(default 0) and exclusive `stop`(default 0, meaning that the last element is included) specify a slice of the array to search.
        ///
        /// Note: out of range errors are treated by rounding the index to the array's start and end. An inverse index range (e.g. from 1 to 0) will return unfound.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrindex
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="jsonScalar"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns>Integer, specifically the position of the scalar value in the array, or -1 if unfound.</returns>
        public static async Task<int> JsonArrayIndexOfAsync(this IDatabase db, RedisKey key, string path, string jsonScalar, int start = 0, int stop = 0) =>
            (int)(await db.ExecuteAsync(GetCommandName(CommandType.Json.ARRINDEX), CombineArguments(key, path, jsonScalar, start, stop)));

        /// <summary>
        /// `JSON.ARRINSERT`
        /// 
        /// Insert the `json` value(s) into the array at `path` before the `index` (shifts to the right).
        ///
        /// The index must be in the array's range. Inserting at `index` 0 prepends to the array. Negative index values are interpreted as starting from the end.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrinsert
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <param name="index"></param>
        /// <param name="json"></param>
        /// <returns>Integer, specifically the array's new size.</returns>
        public static async Task<int> JsonArrayInsertAsync(this IDatabase db, RedisKey key, string path, int index, params string[] json) =>
            (int)(await db.ExecuteAsync(GetCommandName(CommandType.Json.ARRINSERT), CombineArguments(key, path, index, json)));

        /// <summary>
        /// `JSON.ARRLEN`
        /// 
        /// Report the length of the JSON Array at `path` in `key`.
        /// 
        /// `path` defaults to root if not provided. If the `key` or `path` do not exist, null is returned.
        /// 
        /// https://oss.redislabs.com/rejson/commands/#jsonarrlen
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns>Integer, specifically the array's length.</returns>
        public static async Task<int?> JsonArrayLengthAsync(this IDatabase db, RedisKey key, string path = ".")
        {
            var result = await db.ExecuteAsync(GetCommandName(CommandType.Json.ARRLEN), CombineArguments(key, path));

            if (result.IsNull)
            {
                return null;
            }
            else
            {
                return (int)result;
            }
        }

        public static Task JsonArrayPopAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonArrayTrimAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonObjectKeysAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonObjectLengthAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonDebugAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }

        public static Task JsonGetRespAsync(this IDatabase db)
        {
            return Task.CompletedTask;
        }
    }
}