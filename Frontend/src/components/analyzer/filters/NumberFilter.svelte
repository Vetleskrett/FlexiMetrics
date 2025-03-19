<script lang="ts">
	import { Input } from 'src/lib/components/ui/input';
	import { onDestroy } from 'svelte';
	import type { Writable } from 'svelte/store';

	export let filterValue: Writable<{ min: number; max: number }>;

	onDestroy(() => filterValue.set({ min: -Infinity, max: Infinity }));
</script>

<div class="flex flex-row items-center gap-1">
	<Input
		id="filter-input"
		type="number"
		bind:value={$filterValue.min}
		placeholder="Min"
		class="min-w-20 border-black font-normal"
	/>

	<p class="text-lg">-</p>

	<Input
		id="filter-input"
		type="number"
		bind:value={$filterValue.max}
		placeholder="Max"
		class="min-w-20 border-black font-normal"
	/>
</div>
