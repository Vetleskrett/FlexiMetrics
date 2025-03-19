<script lang="ts">
	import type { AnalysisFieldType } from 'src/types';
	import { getCell } from './cells';
	import { Render } from 'svelte-headless-table';

	export let field: {
		value: any;
	};
	export let subType: AnalysisFieldType;

	const renderer = getCell(subType);
</script>

{#if subType == 'Range' || subType == 'Json'}
	<div class="flex flex-col gap-2">
		{#each field?.value as v}
			<Render
				of={renderer({
					value: {
						value: v
					}
				})}
			/>
		{/each}
	</div>
{:else}
	<ul>
		{#each field?.value as v}
			<li class="flex gap-2">
				• <Render
					of={renderer({
						value: {
							value: v
						}
					})}
				/>
			</li>
		{/each}
	</ul>
{/if}
