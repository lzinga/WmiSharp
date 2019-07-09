using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;
using WmiSharp.Attributes;

namespace WmiSharp
{
    public class Wmi
    {
        public static class Namespaces
        {
            public const string CimV2 = @"root\cimv2";
        }


        private ManagementScope scope { get; set; }

        /// <summary>
        /// Creates a WMI instance targeting the current host in the <see cref="Namespaces.CimV2"/> namespace using the executing credentials.
        /// </summary>
        public Wmi()
        {
            new ManagementScope(Namespaces.CimV2);
            scope.Options = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate
            };
        }

        /// <summary>
        /// Creates a WMI instance targeting <paramref name="host"/> using the <paramref name="username"/> and <paramref name="password"/> passed in.
        /// </summary>
        /// <param name="namespace">The WMI namespace to query.</param>
        /// <param name="host">The host to remotely connect to.</param>
        /// <param name="username">The username to connect to the <paramref name="host"/></param>
        /// <param name="password">The password for <paramref name="username"/></param>
        public Wmi(string @namespace, string host, string username, string password)
        {
            scope = new ManagementScope($@"\\{host}\{@namespace}");
            scope.Options = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Default,
                Username = username,
                Password = password
            };
        }

        /// <summary>
        /// Creates a WMI instance targeting <paramref name="host"/> using the executing credentials.
        /// </summary>
        /// <param name="namespace">The WMI namespace to query.</param>
        /// <param name="host">The host to remotely connect to.</param>
        public Wmi(string @namespace, string host) : this()
        {
            scope = new ManagementScope($@"\\{host}\{@namespace}");
            scope.Options = new ConnectionOptions
            {
                Impersonation = ImpersonationLevel.Impersonate
            };
        }

        /// <summary>
        /// Query a single record of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="namespace">The wmi namespace to query.
        /// <param name="query">What to select using WMI Query Language</param>
        /// <returns></returns>
        public T QuerySingleOrDefault<T>(string @namespace, string query)
        {
            if(!query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"The query is not a select statement.");
            }

            T result = default(T);
            scope.Path = new ManagementPath(@namespace);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery(query)))
            using (ManagementObjectCollection wmiRes = searcher.Get())
            {
                foreach (ManagementObject mo in wmiRes)
                {
                    result = WmiHelper.LoadType<T>(mo);
                    break;
                }
            }

            return result;
        }

        public T QuerySingleOrDefault<T>()
        {

            string className = WmiHelper.GetClassName<T>();

            if (string.IsNullOrEmpty(className))
            {
                throw new NullReferenceException($"Type of {typeof(T)} does not use {typeof(WmiClassAttribute)}. This is required.");
            }

            string searchableProperties = WmiHelper.GetSearchableProperties<T>();

            return this.QuerySingleOrDefault<T>(WmiHelper.GetNamespace<T>(), $"SELECT {searchableProperties} FROM {className}");
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>()
        {
            return await Task.Run(() => this.QuerySingleOrDefault<T>());
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string @namespace, string query)
        {
            return await Task.Run(() => this.QuerySingleOrDefault<T>(@namespace, query));
        }

        public IEnumerable<T> Query<T>(string @namespace, string query)
        {
            if (!query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"The query is not a select statement.");
            }


            scope.Path = new ManagementPath(@namespace);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery(query)))
            using (ManagementObjectCollection wmiRes = searcher.Get())
            {
                foreach (ManagementObject mo in wmiRes)
                {
                    yield return WmiHelper.LoadType<T>(mo);
                }
            }
        }

        public IEnumerable<T> Query<T>()
        {
            string className = WmiHelper.GetClassName<T>();

            if (string.IsNullOrEmpty(className))
            {
                throw new NullReferenceException($"Type of {typeof(T)} does not use {typeof(WmiClassAttribute)}. This is required.");
            }

            string searchableProperties = WmiHelper.GetSearchableProperties<T>();
            return this.Query<T>(WmiHelper.GetNamespace<T>(), $"SELECT {searchableProperties} FROM {className}");
        }

        public async Task<IEnumerable<T>> QueryAsync<T>()
        {
            return await Task.Run(() => this.Query<T>());
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string @namespace, string query)
        {
            return await Task.Run(() => this.Query<T>(@namespace, query));
        }







    }
}
