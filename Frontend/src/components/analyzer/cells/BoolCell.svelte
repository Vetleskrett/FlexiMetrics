<script lang="ts">
	import { Check, X } from 'lucide-svelte';

	export let field: {
		value: any | undefined;
	};
</script>

<div class="flex gap-2">
	{#if field.value}
		<Check color="green" /> True
	{:else}
		<X color="red" /> False
	{/if}
</div>
