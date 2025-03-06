<script lang="ts">
	import { Input } from 'src/lib/components/ui/input';
	import { onDestroy } from 'svelte';
	import type { Writable } from 'svelte/store';

	export let filterValue: Writable<string>;

	onDestroy(() => filterValue.set(''));
</script>

<Input
	id="filter-input"
	type="text"
	bind:value={$filterValue}
	placeholder="Filter..."
	class="border-black font-normal"
/>
