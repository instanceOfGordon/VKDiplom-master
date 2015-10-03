using System;
using System.Collections;
using System.Collections.Generic;
using HermiteInterpolation.MathFunctions;
using HermiteInterpolation.Numerics;
using HermiteInterpolation.Shapes.SplineInterpolation;

namespace HermiteInterpolation.SplineKnots
{
    public delegate KnotMatrix KnotsOperation(KnotMatrix leftOp, KnotMatrix rightOp);
    public sealed class ChainedKnotsGenerator : KnotsGenerator,IEnumerable<KnotsGenerator>
    {
        private readonly IList<KnotsGenerator> _generatorChain;
        private readonly IList<KnotsOperation> _operatorChain;

        //public ChainedKnotsGenerator(IDictionary<KnotsGenerator, KnotsOperation> generatorChain)
        //{
        //    _generatorChain = generatorChain;
        //}

        public ChainedKnotsGenerator(KnotsGenerator first)
        {
            _generatorChain = new List<KnotsGenerator> {first};
            _operatorChain = new List<KnotsOperation> {null};
        }

        //public ChainedKnotsGenerator(MathExpression expression) : base(expression)
        //{
        //}

        public override KnotMatrix GenerateKnots(SurfaceDimension uDimension, SurfaceDimension vDimension)
        {
            var result= _generatorChain[0].GenerateKnots(uDimension,vDimension);
            for (int i = 1; i < _operatorChain.Count; i++)
            {
                if(_operatorChain[i]==null) continue;
                result = _operatorChain[i](result, _generatorChain[i].GenerateKnots(uDimension, vDimension));
            }
            return result;
        }

        public void Add(KnotsGenerator generator, KnotsOperation operation)
        {
            _generatorChain.Add(generator);
            _operatorChain.Add(operation);
        }

        //public bool RemoveAt(int index)
        //{
        //    var
        //    _operatorChain.RemoveAt(index);
        //}

        //public IEnumerator<KeyValuePair<KnotsGenerator, KnotsOperation>> GetEnumerator()
        //{
        //   return _generatorChain.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        //public void Add(KeyValuePair<KnotsGenerator, KnotsOperation> item)
        //{
        //    if(_generatorChain.Count==0||item.Value!=null)
        //        _generatorChain.Add(item);
            
        //}

        //public void Clear()
        //{
        //    _generatorChain.Clear();
        //}

        //public bool Contains(KeyValuePair<KnotsGenerator, KnotsOperation> item)
        //{
        //   return _generatorChain.Contains(item);
        //}

        //public void CopyTo(KeyValuePair<KnotsGenerator, KnotsOperation>[] array, int arrayIndex)
        //{
        //    _generatorChain.CopyTo(array,arrayIndex);
        //}

        //public bool Remove(KeyValuePair<KnotsGenerator, KnotsOperation> item)
        //{
        //   return _generatorChain.Remove(item);
        //}

        //public int Count => _generatorChain.Count;

        //public bool IsReadOnly => _generatorChain.IsReadOnly;
        //public bool ContainsKey(KnotsGenerator key)
        //{
        //    return _generatorChain.ContainsKey(key);
        //}

        //public void Add(KnotsGenerator key, KnotsOperation value)
        //{
        //    _generatorChain.Add(key,value);
        //}

        //public bool Remove(KnotsGenerator key)
        //{
        //    return _generatorChain.Remove(key);
        //}

        //public bool TryGetValue(KnotsGenerator key, out KnotsOperation value)
        //{
        //    return _generatorChain.TryGetValue(key,out value);
        //}

        //public KnotsOperation this[KnotsGenerator key]
        //{
        //    get { return _generatorChain[key]; }
        //    set { _generatorChain[key] = value; }
        //}

        //public ICollection<KnotsGenerator> Keys => _generatorChain.Keys;
        //public ICollection<KnotsOperation> Values => _generatorChain.Values;
        public IEnumerator<KnotsGenerator> GetEnumerator()
        {
            return _generatorChain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}