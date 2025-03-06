<script lang="ts">
	import type { AssignmentFieldType } from 'src/types/';
	import { ArrowDownToLine } from 'lucide-svelte';
	import Check from 'lucide-svelte/icons/check';
	import X from 'lucide-svelte/icons/x';
	import DeliveryFieldValue from './DeliveryFieldValue.svelte';

	export let id: string;
	export let value: any;
	export let type: AssignmentFieldType;
	export let subType: AssignmentFieldType | undefined = undefined;
</script>

{#if type == 'ShortText'}
	<h1>{value}</h1>
{:else if type == 'LongText'}
	<p style="white-space: pre-wrap" class="min-w-48">{value}</p>
{:else if type == 'Integer'}
	<h1>{value}</h1>
{:else if type == 'Float'}
	<h1>{value.toFixed(3)}</h1>
{:else if type == 'Boolean'}
	<div class="flex gap-2">
		{#if value}
			<Check color="green" /> True
		{:else}
			<X color="red" /> False
		{/if}
	</div>
{:else if type == 'URL'}
	<a class="text-blue-500" target="_blank" href={value}>{value}</a>
{:else if type == 'JSON'}
	<div class="min-w-48 rounded bg-gray-100 p-2 font-mono">
		<p style="white-space: pre-wrap">
			{JSON.stringify(JSON.parse(value), null, 2)}
		</p>
	</div>
{:else if type == 'File'}
	<a class="flex items-center text-blue-500" download href={`/api/delivery-fields/${id}`}>
		<ArrowDownToLine size="20" />
		{value?.FileName}
	</a>
{:else if type == 'List' && subType}
	<ul>
		{#each value as item}
			<li>
				• <DeliveryFieldValue {id} value={item} type={subType} />
			</li>
		{/each}
	</ul>
{:else}
	<h1>{value}</h1>
{/if}
