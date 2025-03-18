<script lang="ts">
	import DatePicker from 'src/components/inputs/DatePicker.svelte';
	import { onDestroy } from 'svelte';
	import type { Writable } from 'svelte/store';
	import { type DateValue } from '@internationalized/date';

	export let filterValue: Writable<{ after?: DateValue; before?: DateValue }>;

	onDestroy(() => filterValue.set({ after: undefined, before: undefined }));
</script>

<div class="flex flex-row items-center gap-1">
	<DatePicker id="filter-input" bind:value={$filterValue.after} />

	<p class="text-lg">-</p>

	<DatePicker id="filter-input" bind:value={$filterValue.before} />
</div>
