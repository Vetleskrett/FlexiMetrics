<script lang="ts">
	import { Progress } from '$lib/components/ui/progress';

	export let field: {
		value: {
			Value: number;
			Max: number;
		};
	};
</script>

<div class="flex items-center justify-center">
	<Progress class="h-6 min-w-20" value={field?.value?.Value} max={field?.value?.Max} />
	<p class="absolute text-nowrap pb-[1px] font-semibold">
		{field?.value?.Value} / {field?.value?.Max}
	</p>
</div>
