<script lang="ts">
	import { Input } from 'src/lib/components/ui/input';
	import type { Readable, Writable } from 'svelte/store';

	export let filterValue: Writable<string>;
</script>

<Input
	id="filter-input"
	type="text"
	bind:value={$filterValue}
	placeholder="Filter..."
	class="border-black font-normal"
/>
