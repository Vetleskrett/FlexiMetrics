<script lang="ts">
	import { ScrollArea as ScrollAreaPrimitive } from 'bits-ui';
	import { cn } from '$lib/utils.js';

	type $$Props = ScrollAreaPrimitive.ScrollbarProps & {
		orientation?: 'vertical' | 'horizontal';
		position?: 'before' | 'after';
	};

	let className: $$Props['class'] = undefined;
	export let orientation: $$Props['orientation'] = 'vertical';
	export let position: $$Props['position'] = 'after';
	export { className as class };
</script>

<ScrollAreaPrimitive.Scrollbar
	{orientation}
	class={cn(
		'flex touch-none select-none transition-colors',
		orientation === 'vertical' && 'h-full w-2.5 border-l border-l-transparent p-px',
		orientation === 'horizontal' && 'h-2.5 w-full border-t border-t-transparent p-px',
		position === 'before' && 'top-0',
		position === 'before' && 'left-0',
		className
	)}
>
	<slot />
	<ScrollAreaPrimitive.Thumb
		class={cn('relative rounded-full bg-border', orientation === 'vertical' && 'flex-1')}
	/>
</ScrollAreaPrimitive.Scrollbar>
