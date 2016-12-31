﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Shared
{
    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
    [SuppressMessage("ReSharper", "UncatchableException")]
    internal static class ExceptionAssert
    {
        [NotNull]
        static E Handle<E>([NotNull] E exception, string expectedParamName = null) where E : Exception
        {
            var argumentException = exception as ArgumentException;
            if (argumentException != null)
            {
                if (string.Equals(expectedParamName ?? "", argumentException.ParamName ?? "", StringComparison.Ordinal))
                {
                    return exception;
                }

                throw new AssertInconclusiveException(
                    string.Format(
                        "{0}<{1}> inconclusive. Expected '{2}': <{3}>. Actual: <{4}>",
                        nameof(Throws),
                        typeof(E).Name,
                        nameof(ArgumentException.ParamName),
                        expectedParamName ?? "(null)",
                        argumentException.ParamName ?? "(null)"));
            }

            if (expectedParamName != null)
            {
                throw new AssertInconclusiveException(
                    string.Format("{0}<{1}> inconclusive. '{2}' is not used.", nameof(Throws), typeof(E).Name, nameof(expectedParamName)));
            }

            return exception;
        }

        [NotNull]
        public static E Throws<E>([NotNull] [InstantHandle] Action action, string expectedParamName = null) where E : Exception
        {
            try
            {
                action();
            }
            catch (E e)
            {
                return Handle(e, expectedParamName);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new AssertFailedException(
                    string.Format(
                        "{0}<{1}> failed. Expected exception: '{1}'. Actual: '{2}' - {3}.",
                        nameof(Throws),
                        typeof(E).Name,
                        e.GetType().Name,
                        e));
            }

            throw new AssertFailedException(string.Format("{0}<{1}> failed. Exception '{1}' not thrown.", nameof(Throws), typeof(E).Name));
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        [ItemNotNull]
        public static async Task<E> Throws<E>([NotNull] [InstantHandle] Func<Task> action, string expectedParamName = null) where E : Exception
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (E e)
            {
                return Handle(e, expectedParamName);
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new AssertFailedException(
                    string.Format(
                        "{0}<{1}> failed. Expected exception: '{1}'. Actual: '{2}' - {3}.",
                        nameof(Throws),
                        typeof(E).Name,
                        e.GetType().Name,
                        e));
            }

            throw new AssertFailedException(string.Format("{0}<{1}> failed. Exception '{1}' not thrown.", nameof(Throws), typeof(E).Name));
        }
    }
}