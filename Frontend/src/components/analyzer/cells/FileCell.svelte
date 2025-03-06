<script lang="ts">
	import ArrowDownToline from 'lucide-svelte/icons/arrow-down-to-line';

	export let value: any;
</script>

<a href="TODO: add url here" download="TODO" class="flex items-center text-blue-500">
	<ArrowDownToline size="20" />
	{value}
</a>
