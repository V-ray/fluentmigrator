#region License
// Copyright (c) 2007-2018, FluentMigrator Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;

using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure.Extensions;
using FluentMigrator.Postgres;

using JetBrains.Annotations;

using Microsoft.Extensions.Options;

namespace FluentMigrator.Runner.Generators.Postgres
{
    public class Postgres10_0Generator : PostgresGenerator
    {
        public Postgres10_0Generator([NotNull] PostgresQuoter quoter)
            : this(quoter, new OptionsWrapper<GeneratorOptions>(new GeneratorOptions()))
        {
        }

        public Postgres10_0Generator([NotNull] PostgresQuoter quoter, [NotNull] IOptions<GeneratorOptions> generatorOptions)
            : base(new Postgres10_0Column(quoter, new Postgres92.Postgres92TypeMap()), quoter, generatorOptions)
        {
        }

        protected Postgres10_0Generator([NotNull] PostgresQuoter quoter, [NotNull] IOptions<GeneratorOptions> generatorOptions, [NotNull] ITypeMap typeMap)
            : base(new Postgres10_0Column(quoter, typeMap), quoter, generatorOptions)
        {
        }

        /// <inheritdoc />
        protected override ICollection<string> GetIndexStorageParameters(CreateIndexExpression expression)
        {
            var parameters = base.GetIndexStorageParameters(expression);

            var buffering = expression.Index.GetAdditionalFeature<GistBuffering?>(PostgresExtensions.IndexBuffering);
            if (buffering.HasValue)
            {
                parameters.Add($"BUFFERING = {buffering.Value.ToString().ToUpper()}");
            }

            var pendingList = expression.Index.GetAdditionalFeature<long?>(PostgresExtensions.IndexGinPendingListLimit);
            if (pendingList.HasValue)
            {
                parameters.Add($"GIN_PENDING_LIST_LIMIT = {pendingList}");
            }

            var perRangePage = expression.Index.GetAdditionalFeature<int?>(PostgresExtensions.IndexPagesPerRange);
            if (perRangePage.HasValue)
            {
                parameters.Add($"PAGES_PER_RANGE = {perRangePage}");
            }

            var autosummarize = expression.Index.GetAdditionalFeature<bool?>(PostgresExtensions.IndexAutosummarize);
            if (autosummarize.HasValue && autosummarize.Value)
            {
                parameters.Add("AUTOSUMMARIZE");
            }

            return parameters;
        }
    }
}
