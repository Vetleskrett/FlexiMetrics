<script lang="ts">
	import Dot from 'lucide-svelte/icons/dot';

	export let values: any[];
</script>

<div class="flex flex-col justify-start">
	{#each values as value}
		<p class="flex items-center"><Dot />{value}</p>
	{/each}
</div>
