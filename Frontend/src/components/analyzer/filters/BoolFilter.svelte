<script lang="ts">
	import { Checkbox } from 'src/lib/components/ui/checkbox';
	import { onDestroy } from 'svelte';
	import type { Writable } from 'svelte/store';

	export let filterValue: Writable<boolean | undefined>;

	onDestroy(() => filterValue.set(undefined));
</script>

<Checkbox bind:checked={$filterValue} />
