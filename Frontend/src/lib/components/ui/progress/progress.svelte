<script lang="ts">
	import { Progress as ProgressPrimitive } from 'bits-ui';
	import { cn } from 'src/lib/utils';

	type $$Props = ProgressPrimitive.Props;
	let className: $$Props['class'] = undefined;
	export let value: $$Props['value'] = 0;
	export let max: $$Props['max'] = 100;
	export { className as class };
</script>

<ProgressPrimitive.Root
	class={cn('relative h-4 w-full overflow-hidden rounded-full bg-[#F2F2F2]', className)}
	{...$$restProps}
>
	<div
		class="h-full w-full flex-1 bg-green-400 transition-all"
		style={`transform: translateX(-${100 - (100 * (value ?? 0)) / (max ?? 1)}%)`}
	></div>
</ProgressPrimitive.Root>
