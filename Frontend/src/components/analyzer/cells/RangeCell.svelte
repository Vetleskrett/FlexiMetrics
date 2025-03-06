<script lang="ts">
	import { Progress } from '$lib/components/ui/progress';

	export let value: number;
	export let max: number;
</script>

<div class="flex items-center justify-center">
	<Progress class="h-6" {value} {max} />
	<p class="absolute pb-[1px] font-semibold">{value} / {max}</p>
</div>
