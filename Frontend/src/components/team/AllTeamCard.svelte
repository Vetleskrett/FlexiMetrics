<script lang="ts">
	import { Role, type Team } from 'src/types/';
	import * as Card from '$lib/components/ui/card';
	import * as Table from '$lib/components/ui/table';
	import { Progress } from '$lib/components/ui/progress';
	import CustomButton from '../CustomButton.svelte';

	export let teams: Team[],
		courseId: string,
		userRole: Role = Role.Teacher,
		onJoin: ((teamId: string) => void) | null = null;
</script>

<Card.Root class="w-full overflow-hidden p-0">
	<Card.Content class="p-0">
		<Table.Root class="h-full">
			<Table.Header>
				<Table.Row>
					<Table.Head class="flex h-8 items-center justify-center px-6 font-bold text-black">
						Team
					</Table.Head>
					<Table.Head class="h-8 px-6 font-bold text-black">Members</Table.Head>
					<Table.Head class="h-8 w-[250px] px-6 font-bold text-black"
						>{userRole == Role.Teacher ? 'Progess' : ''}</Table.Head
					>
				</Table.Row>
			</Table.Header>
			<Table.Body>
				{#each teams as team}
					{#if userRole == Role.Teacher}
						<a
							href="/teacher/courses/{courseId}/teams/{team.id}"
							class="table-row hover:bg-blue-50"
						>
							<Table.Cell class="flex h-full items-center justify-center px-6">
								<p>{team.teamNr}</p>
							</Table.Cell>
							<Table.Cell class="px-6">
								{#each team.students as student}
									<p class="whitespace-nowrap">• {student.name}</p>
								{/each}
							</Table.Cell>
							<Table.Cell class="w-full px-6">
								<Progress value={team.complete} />
							</Table.Cell>
						</a>
					{:else}
						<Table.Row>
							<Table.Cell class="flex h-full items-center justify-center px-6">
								<p>{team.teamNr}</p>
							</Table.Cell>
							<Table.Cell class="px-6">
								{#each team.students as student}
									<p>• {student.email}</p>
								{/each}
							</Table.Cell>
							<Table.Cell class="px-6">
								<CustomButton
									on:click={() => (onJoin ? onJoin(team.id) : console.log('No function'))}
									color={'green'}
								>
									<p>Join team {team.teamNr}</p>
								</CustomButton>
							</Table.Cell>
						</Table.Row>
					{/if}
				{/each}
			</Table.Body>
		</Table.Root>
	</Card.Content>
</Card.Root>
