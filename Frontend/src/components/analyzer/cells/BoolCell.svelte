<script lang="ts">
	import { Check, X } from 'lucide-svelte';

	export let value: boolean;
</script>

<div class="flex gap-2">
	{#if value}
		<Check color="green" /> True
	{:else}
		<X color="red" /> False
	{/if}
</div>
