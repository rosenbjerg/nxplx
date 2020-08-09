using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NxPlx.Services.Index
{
    class FunctionCache<TArg1, TResult>
    {
        private readonly Func<TArg1, Task<TResult>> _function;
        private readonly Dictionary<TArg1, TResult> _dictionary = new Dictionary<TArg1, TResult>();

        public FunctionCache(Func<TArg1, Task<TResult>> function)
        {
            _function = function;
        }

        public async ValueTask<TResult> Invoke(TArg1 arg1)
        {
            TResult result;
            if (!_dictionary.TryGetValue(arg1, out result))
            {
                result = await _function(arg1);
                _dictionary[arg1] = result;
            }
            return result;
        }
    }
    class FunctionCache<TArg1, TArg2, TResult>
    {
        private readonly Func<TArg1, TArg2, Task<TResult>> _function;
        private readonly Dictionary<(TArg1, TArg2), TResult> _dictionary = new Dictionary<(TArg1, TArg2), TResult>();

        public FunctionCache(Func<TArg1, TArg2, Task<TResult>> function)
        {
            _function = function;
        }

        public async ValueTask<TResult> Invoke(TArg1 arg1, TArg2 arg2)
        {
            TResult result;
            var key = (arg1, arg2);
            if (!_dictionary.TryGetValue(key, out result))
            {
                result = await _function(arg1, arg2);
                _dictionary[key] = result;
            }
            return result;
        }
    }
}