<script lang="ts">
	import { ArrowDownToLine } from 'lucide-svelte';

	export let field: {
		value: any;
		id: string;
	};
</script>

<a class="flex items-center text-blue-500" download href={`/api/analysis-fields/${field?.id}`}>
	<ArrowDownToLine size="20" />
	{field?.value?.FileName}
</a>
