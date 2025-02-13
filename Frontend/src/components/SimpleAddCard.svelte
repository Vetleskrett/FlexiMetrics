<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Plus from 'lucide-svelte/icons/plus';
	import Button from '$lib/components/ui/button/button.svelte';
	import { Input } from 'src/lib/components/ui/input';
	import CustomButton from './CustomButton.svelte';

	export let headline: string,
		inputString: string,
		inputType: string,
		actionString: string,
		addFunction: (input: any) => void;
	let input: any;

	function handleInput(thisInput: Event) {
		input = (thisInput.target as HTMLInputElement).value;
	}
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Header class="mb-6 flex flex-row items-center justify-between">
		<div class="flex items-center">
			<img width="48" height="48" src="https://img.icons8.com/fluency/480/add--v1.png" alt="add" />
			<Card.Title class="ml-4 text-3xl">{headline}</Card.Title>
		</div>
	</Card.Header>
	<Card.Content class="p-0">
		<div class="flex flex-col">
			<Separator class="w-full" />
			<div class="mx-8 my-6 flex items-end gap-4">
				<div class="w-full">
					<label class="text-sm font-bold" for="AddInput">{inputString}</label>
					<Input id="AddInput" placeholder={inputString} type={inputType} on:input={handleInput} />
				</div>
				<CustomButton color="green" on:click={() => addFunction(input)}>
					<Plus />
					<p>{actionString}</p>
				</CustomButton>
			</div>
		</div>
	</Card.Content>
</Card.Root>
