<script lang="ts">
	import type { AnalysisFieldType } from 'src/types';
	import { getCell } from './cells';
	import { Render } from 'svelte-headless-table';

	export let value: any;
	export let subType: AnalysisFieldType;

	const renderer = getCell(subType);
</script>

{#if subType == 'Range' || subType == 'Json'}
	<div class="flex flex-col gap-2">
		{#each value as v}
			<Render of={renderer({ value: v })} />
		{/each}
	</div>
{:else}
	<ul>
		{#each value as v}
			<li class="flex gap-2">
				• <Render of={renderer({ value: v })} />
			</li>
		{/each}
	</ul>
{/if}
