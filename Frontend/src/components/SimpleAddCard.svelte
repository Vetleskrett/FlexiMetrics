<script lang="ts">
	import * as Card from '$lib/components/ui/card/index.js';
	import { Separator } from '$lib/components/ui/separator/index.js';
	import Plus from 'lucide-svelte/icons/plus';
	import Button from '$lib/components/ui/button/button.svelte';

	export let headline: string, inputString: string, inputType: string, actionString: string, addFunction: (input: any) => void;
	let input: any;

	function handleInput(thisInput: Event){
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
			<div class="m-8 flex items-center px-6 text-sm font-bold">
				<div>
					<label class="pl-2" for="AddInput">{inputString}</label>
					<input
						id="AddInput"
						class="mr-2 h-9 rounded-md border-2 border-black p-2"
						placeholder={inputString}
						type={inputType}
						on:input={handleInput}
					/>
				</div>
				<Button
					class="mt-auto flex h-9 flex-row justify-between gap-2 bg-button-green pl-2 pr-3 hover:bg-button-green-hover"
					on:click={() => addFunction(input)}
				>
					<Plus />
					<p>{actionString}</p>
				</Button>
			</div>
		</div>
	</Card.Content>
</Card.Root>
