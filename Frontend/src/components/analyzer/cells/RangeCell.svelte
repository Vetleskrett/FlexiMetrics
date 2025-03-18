<script lang="ts">
	import { Progress } from '$lib/components/ui/progress';

	export let value: {
		Value: number;
		Max: number;
	};
</script>

<div class="flex items-center justify-center">
	<Progress class="h-6" value={value.Value} max={value.Max} />
	<p class="absolute text-nowrap pb-[1px] font-semibold">{value.Value} / {value.Max}</p>
</div>
