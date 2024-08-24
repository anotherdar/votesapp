import { create } from 'zustand'

type Store = {
    votesInfo: VotesInfo,
    setVoteInfo: (votesInfo: Store['votesInfo']) => void
}

export const store = create<Store>((set) => ({
    votesInfo: {} as VotesInfo,
    setVoteInfo(votesInfo) {
        set({votesInfo})
    },
}))


export const useVotesInfo = () => {
    return store(({votesInfo, setVoteInfo}) => ({ votesInfo, setVoteInfo}))
}